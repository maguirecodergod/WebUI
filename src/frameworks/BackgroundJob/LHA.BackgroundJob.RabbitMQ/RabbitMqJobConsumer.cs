using System.Text;
using LHA.MessageBroker.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LHA.BackgroundJob.RabbitMQ;

/// <summary>
/// Background service that consumes background job messages from RabbitMQ queues.
/// For each registered job type, it declares a queue bound to the background job exchange,
/// deserializes args, and delegates to <see cref="IBackgroundJobExecuter"/>.
/// Supports dead-letter routing for failed jobs and automatic reconnection.
/// </summary>
public sealed class RabbitMqJobConsumer : BackgroundService
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly IBackgroundJobSerializer _serializer;
    private readonly IBackgroundJobExecuter _executer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundJobOptions _jobOptions;
    private readonly RabbitMqBackgroundJobOptions _rabbitOptions;
    private readonly ILogger<RabbitMqJobConsumer> _logger;

    public RabbitMqJobConsumer(
        RabbitMqConnectionManager connectionManager,
        IBackgroundJobSerializer serializer,
        IBackgroundJobExecuter executer,
        IServiceScopeFactory scopeFactory,
        IOptions<BackgroundJobOptions> jobOptions,
        IOptions<RabbitMqBackgroundJobOptions> rabbitOptions,
        ILogger<RabbitMqJobConsumer> logger)
    {
        _connectionManager = connectionManager;
        _serializer = serializer;
        _executer = executer;
        _scopeFactory = scopeFactory;
        _jobOptions = jobOptions.Value;
        _rabbitOptions = rabbitOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_jobOptions.IsJobExecutionEnabled)
        {
            _logger.LogWarning("Background job execution is disabled. RabbitMQ consumer will not start.");
            return;
        }

        var registeredJobs = _jobOptions.GetJobs();
        if (registeredJobs.Count == 0)
        {
            _logger.LogWarning("No background jobs registered. RabbitMQ consumer will not start.");
            return;
        }

        _logger.LogInformation(
            "Starting RabbitMQ background job consumer for {JobCount} job type(s).",
            registeredJobs.Count);

        // Reconnect loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(registeredJobs, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "RabbitMQ background job consumer failed. Reconnecting in {Delay}ms...",
                    _rabbitOptions.ReconnectDelayMs);
                await Task.Delay(_rabbitOptions.ReconnectDelayMs, stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(
        IReadOnlyList<BackgroundJobConfiguration> jobs,
        CancellationToken stoppingToken)
    {
        var channel = await _connectionManager.CreateConsumerChannelAsync(stoppingToken);

        try
        {
            // QoS
            await channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _rabbitOptions.PrefetchCount,
                global: false,
                cancellationToken: stoppingToken);

            // Declare topology for each job type
            await DeclareTopologyAsync(channel, jobs, stoppingToken);

            // Set up a consumer per queue
            foreach (var job in jobs)
            {
                var queueName = GetQueueName(job.JobName);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, args) =>
                {
                    await ProcessMessageAsync(channel, args, stoppingToken);
                };

                await channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumerTag: $"lha-bg-{job.JobName}-{Environment.MachineName}",
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "Consuming background jobs from queue [{Queue}] for job [{JobName}].",
                    queueName, job.JobName);
            }

            // Block until cancellation — let the async consumers do their work
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        finally
        {
            await channel.CloseAsync(cancellationToken: CancellationToken.None);
            channel.Dispose();
        }
    }

    private async Task DeclareTopologyAsync(
        IChannel channel,
        IReadOnlyList<BackgroundJobConfiguration> jobs,
        CancellationToken cancellationToken)
    {
        // Main exchange
        await channel.ExchangeDeclareAsync(
            exchange: _rabbitOptions.ExchangeName,
            type: _rabbitOptions.ExchangeType,
            durable: _rabbitOptions.Durable,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // Dead-letter exchange
        await channel.ExchangeDeclareAsync(
            exchange: _rabbitOptions.DeadLetterExchange,
            type: "direct",
            durable: _rabbitOptions.Durable,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // Per-job queue declarations and bindings
        foreach (var job in jobs)
        {
            var queueName = GetQueueName(job.JobName);
            var dlqName = $"{queueName}.dlq";

            // Dead-letter queue
            await channel.QueueDeclareAsync(
                queue: dlqName,
                durable: _rabbitOptions.Durable,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: dlqName,
                exchange: _rabbitOptions.DeadLetterExchange,
                routingKey: job.JobName,
                cancellationToken: cancellationToken);

            // Main queue with DLX
            var queueArgs = new Dictionary<string, object?>
            {
                ["x-dead-letter-exchange"] = _rabbitOptions.DeadLetterExchange,
                ["x-dead-letter-routing-key"] = job.JobName
            };

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: _rabbitOptions.Durable,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: _rabbitOptions.ExchangeName,
                routingKey: job.JobName,
                cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(
        IChannel channel,
        BasicDeliverEventArgs args,
        CancellationToken stoppingToken)
    {
        var deliveryTag = args.DeliveryTag;

        try
        {
            // Extract job name from header
            var jobName = GetHeaderString(args.BasicProperties, "lha.bg.jobName");
            if (string.IsNullOrEmpty(jobName))
            {
                _logger.LogWarning("Received message without job name header. Rejecting.");
                await channel.BasicRejectAsync(deliveryTag, requeue: false);
                return;
            }

            if (!_jobOptions.TryGetJob(jobName, out var config) || config is null)
            {
                _logger.LogWarning(
                    "No job registered for name [{JobName}]. Rejecting.", jobName);
                await channel.BasicRejectAsync(deliveryTag, requeue: false);
                return;
            }

            var serializedArgs = Encoding.UTF8.GetString(args.Body.Span);
            var jobArgs = _serializer.Deserialize(serializedArgs, config.ArgsType);

            await using var scope = _scopeFactory.CreateAsyncScope();

            var context = new JobExecutionContext
            {
                ServiceProvider = scope.ServiceProvider,
                JobType = config.JobType,
                ArgsType = config.ArgsType,
                JobArgs = jobArgs,
                CancellationToken = stoppingToken
            };

            _logger.LogDebug(
                "Processing background job [{JobName}] from RabbitMQ. MessageId: {MessageId}.",
                jobName, args.BasicProperties.MessageId);

            await _executer.ExecuteAsync(context);

            await channel.BasicAckAsync(deliveryTag, multiple: false);

            _logger.LogDebug("Background job [{JobName}] completed. Acked.", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process background job message. DeliveryTag: {DeliveryTag}.",
                deliveryTag);

            // Check retry count via x-death header
            var deathCount = GetDeathCount(args.BasicProperties);
            if (deathCount >= _rabbitOptions.MaxDeliveryAttempts)
            {
                _logger.LogError(
                    "Message exceeded max delivery attempts ({Max}). Sending to DLQ.",
                    _rabbitOptions.MaxDeliveryAttempts);
                await channel.BasicRejectAsync(deliveryTag, requeue: false);
            }
            else
            {
                // Reject and requeue for retry
                await channel.BasicNackAsync(deliveryTag, multiple: false, requeue: true);
            }
        }
    }

    private string GetQueueName(string jobName) =>
        $"{_rabbitOptions.QueuePrefix}.{jobName}";

    private static string? GetHeaderString(IReadOnlyBasicProperties properties, string key)
    {
        if (properties.Headers is null) return null;
        if (!properties.Headers.TryGetValue(key, out var value)) return null;

        return value switch
        {
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string s => s,
            _ => value?.ToString()
        };
    }

    private static int GetDeathCount(IReadOnlyBasicProperties properties)
    {
        if (properties.Headers is null) return 0;
        if (!properties.Headers.TryGetValue("x-death", out var xDeath)) return 0;

        if (xDeath is IList<object> deathList && deathList.Count > 0)
        {
            if (deathList[0] is IDictionary<string, object> entry &&
                entry.TryGetValue("count", out var count))
            {
                return Convert.ToInt32(count);
            }
        }

        return 0;
    }
}
