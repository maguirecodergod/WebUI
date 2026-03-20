using System.Text;
using LHA.MessageBroker.RabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LHA.BackgroundJob.RabbitMQ;

/// <summary>
/// <see cref="IBackgroundJobManager"/> implementation that publishes job args
/// to a RabbitMQ queue. Jobs are consumed by <see cref="RabbitMqJobConsumer"/>.
/// Each job type is routed to its own queue via routing key = job name.
/// </summary>
public sealed class RabbitMqBackgroundJobManager : IBackgroundJobManager
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly IBackgroundJobSerializer _serializer;
    private readonly BackgroundJobOptions _jobOptions;
    private readonly RabbitMqBackgroundJobOptions _rabbitOptions;
    private readonly ILogger<RabbitMqBackgroundJobManager> _logger;

    private bool _topologyDeclared;
    private readonly SemaphoreSlim _topologyLock = new(1, 1);

    public RabbitMqBackgroundJobManager(
        RabbitMqConnectionManager connectionManager,
        IBackgroundJobSerializer serializer,
        IOptions<BackgroundJobOptions> jobOptions,
        IOptions<RabbitMqBackgroundJobOptions> rabbitOptions,
        ILogger<RabbitMqBackgroundJobManager> logger)
    {
        _connectionManager = connectionManager;
        _serializer = serializer;
        _jobOptions = jobOptions.Value;
        _rabbitOptions = rabbitOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> EnqueueAsync<TArgs>(
        TArgs args,
        CBackgroundJobPriority priority = CBackgroundJobPriority.Normal,
        TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(args);

        var config = _jobOptions.GetJob(typeof(TArgs));
        var serializedArgs = _serializer.Serialize(args);
        var messageId = Guid.CreateVersion7().ToString("N");

        var channel = await _connectionManager.GetPublishChannelAsync();

        await EnsureTopologyAsync(channel, config.JobName);

        // Build message properties
        var properties = new BasicProperties
        {
            MessageId = messageId,
            ContentType = "application/json",
            DeliveryMode = _rabbitOptions.Durable ? DeliveryModes.Persistent : DeliveryModes.Transient,
            Headers = new Dictionary<string, object?>
            {
                ["lha.bg.jobName"] = config.JobName,
                ["lha.bg.argsType"] = typeof(TArgs).AssemblyQualifiedName,
                ["lha.bg.priority"] = (int)priority,
                ["lha.bg.enqueueTime"] = DateTimeOffset.UtcNow.ToString("O")
            }
        };

        if (delay.HasValue && delay.Value > TimeSpan.Zero)
        {
            // Use per-message TTL + dead-letter routing for delayed jobs
            // This requires the delayed queue pattern or a delay plugin
            properties.Expiration = ((long)delay.Value.TotalMilliseconds).ToString();
        }

        var body = Encoding.UTF8.GetBytes(serializedArgs);
        var routingKey = config.JobName;

        await channel.BasicPublishAsync(
            exchange: _rabbitOptions.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation(
            "Published background job {JobName} to RabbitMQ. MessageId: {MessageId}, Delay: {Delay}.",
            config.JobName, messageId, delay);

        return messageId;
    }

    /// <summary>
    /// Declares the exchange and queue topology once per process lifetime.
    /// </summary>
    private async Task EnsureTopologyAsync(IChannel channel, string jobName)
    {
        if (_topologyDeclared) return;

        await _topologyLock.WaitAsync();
        try
        {
            if (_topologyDeclared) return;

            // Declare the main exchange
            await channel.ExchangeDeclareAsync(
                exchange: _rabbitOptions.ExchangeName,
                type: _rabbitOptions.ExchangeType,
                durable: _rabbitOptions.Durable,
                autoDelete: false);

            // Declare the dead-letter exchange
            await channel.ExchangeDeclareAsync(
                exchange: _rabbitOptions.DeadLetterExchange,
                type: "direct",
                durable: _rabbitOptions.Durable,
                autoDelete: false);

            _topologyDeclared = true;

            _logger.LogDebug(
                "RabbitMQ background job topology declared. Exchange: {Exchange}, DLX: {DLX}.",
                _rabbitOptions.ExchangeName, _rabbitOptions.DeadLetterExchange);
        }
        finally
        {
            _topologyLock.Release();
        }
    }
}
