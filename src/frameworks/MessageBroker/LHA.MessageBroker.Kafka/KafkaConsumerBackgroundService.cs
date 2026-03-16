using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Background service that continuously consumes messages from Kafka topics,
/// deserializes them, and dispatches to the appropriate <see cref="IMessageHandler{T}"/>.
/// Supports multi-tenant filtering, dead-letter queue, and retry with exponential backoff.
/// </summary>
/// <typeparam name="T">The message payload type.</typeparam>
public sealed class KafkaConsumerBackgroundService<T> : BackgroundService where T : class
{
    private readonly KafkaConnectionFactory _connectionFactory;
    private readonly IMessageSerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaConsumerBackgroundService<T>> _logger;
    private readonly string _topic;
    private readonly string? _tenantIdFilter;

    public KafkaConsumerBackgroundService(
        KafkaConnectionFactory connectionFactory,
        IMessageSerializer serializer,
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaOptions> options,
        ILogger<KafkaConsumerBackgroundService<T>> logger,
        string topic,
        string? tenantIdFilter = null)
    {
        _connectionFactory = connectionFactory;
        _serializer = serializer;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
        _topic = topic;
        _tenantIdFilter = tenantIdFilter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _options.Topics.TryGetValue(_topic, out var topicOptions);
        topicOptions ??= new KafkaTopicOptions();
        var groupId = topicOptions.GroupId ?? _options.Consumer.GroupId;

        _logger.LogInformation(
            "Starting Kafka consumer for topic [{Topic}] group [{GroupId}] type [{MessageType}]",
            _topic, groupId, typeof(T).Name);

        var consumer = _connectionFactory.CreateConsumer(groupId);

        try
        {
            consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result?.Message is null) continue;

                    await ProcessMessageAsync(consumer, result, topicOptions, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex,
                        "Kafka consume error on topic [{Topic}]: {Error}",
                        _topic, ex.Error.Reason);

                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Kafka consumer for [{Topic}] shutting down", _topic);
        }
        finally
        {
            consumer.Close();
            consumer.Dispose();
            _logger.LogInformation("Kafka consumer for [{Topic}] disposed", _topic);
        }
    }

    private async Task ProcessMessageAsync(
        IConsumer<string, byte[]> consumer,
        ConsumeResult<string, byte[]> result,
        KafkaTopicOptions topicOptions,
        CancellationToken stoppingToken)
    {
        var headers = ExtractHeaders(result.Message.Headers);

        // Tenant filtering: skip messages not belonging to the target tenant
        if (!string.IsNullOrEmpty(_tenantIdFilter))
        {
            var messageTenantId = headers.GetValueOrDefault(MessageHeaders.TenantId);
            if (!string.Equals(messageTenantId, _tenantIdFilter, StringComparison.OrdinalIgnoreCase))
            {
                consumer.Commit(result);
                return;
            }
        }

        var context = BuildContext(result, headers);

        _logger.LogDebug(
            "Processing message from {Topic}[{Partition}]@{Offset} | TenantId={TenantId}",
            result.Topic, result.Partition.Value, result.Offset.Value,
            context.TenantId);

        var success = false;
        var maxRetries = topicOptions.MaxRetryAttempts;

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<T>>();

                success = await handler.HandleAsync(
                    context with { RetryAttempt = attempt },
                    stoppingToken);

                if (success) break;

                _logger.LogWarning(
                    "Handler returned false for {Topic}[{Partition}]@{Offset} attempt {Attempt}/{MaxRetries}",
                    result.Topic, result.Partition.Value, result.Offset.Value,
                    attempt + 1, maxRetries + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Handler exception for {Topic}[{Partition}]@{Offset} attempt {Attempt}/{MaxRetries}",
                    result.Topic, result.Partition.Value, result.Offset.Value,
                    attempt + 1, maxRetries + 1);
            }

            if (attempt < maxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(
                    topicOptions.RetryBackoffBaseMs * Math.Pow(2, attempt));
                await Task.Delay(delay, stoppingToken);
            }
        }

        if (!success && !string.IsNullOrEmpty(topicOptions.DeadLetterTopic))
        {
            await PublishToDeadLetterAsync(result, topicOptions.DeadLetterTopic, headers);
        }

        // Always commit to avoid blocking (failed messages go to DLQ)
        consumer.Commit(result);
    }

    private MessageContext<T> BuildContext(
        ConsumeResult<string, byte[]> result,
        IReadOnlyDictionary<string, string> headers)
    {
        var payload = _serializer.Deserialize<T>(result.Message.Value)
            ?? throw new InvalidOperationException(
                $"Failed to deserialize message from {result.Topic}[{result.Partition}]@{result.Offset}");

        return new MessageContext<T>
        {
            Payload = payload,
            TenantId = headers.GetValueOrDefault(MessageHeaders.TenantId),
            CorrelationId = headers.GetValueOrDefault(MessageHeaders.CorrelationId),
            CausationId = headers.GetValueOrDefault(MessageHeaders.CausationId),
            UserId = headers.GetValueOrDefault(MessageHeaders.UserId),
            Source = headers.GetValueOrDefault(MessageHeaders.Source),
            SchemaVersion = headers.GetValueOrDefault(MessageHeaders.SchemaVersion),
            Destination = result.Topic,
            Timestamp = result.Message.Timestamp.Type != TimestampType.NotAvailable
                ? DateTimeOffset.FromUnixTimeMilliseconds(result.Message.Timestamp.UnixTimestampMs)
                : DateTimeOffset.UtcNow,
            Headers = headers,
            BrokerMetadata = new Dictionary<string, object>
            {
                [KafkaMetadataKeys.Topic] = result.Topic,
                [KafkaMetadataKeys.Partition] = result.Partition.Value,
                [KafkaMetadataKeys.Offset] = result.Offset.Value
            }
        };
    }

    private async Task PublishToDeadLetterAsync(
        ConsumeResult<string, byte[]> result,
        string deadLetterTopic,
        IReadOnlyDictionary<string, string> headers)
    {
        try
        {
            var producer = _connectionFactory.GetProducer();
            var dlqHeaders = new Headers();

            foreach (var kvp in headers)
                dlqHeaders.Add(kvp.Key, Encoding.UTF8.GetBytes(kvp.Value));

            dlqHeaders.Add("x-original-topic", Encoding.UTF8.GetBytes(result.Topic));
            dlqHeaders.Add("x-original-partition", Encoding.UTF8.GetBytes(result.Partition.Value.ToString()));
            dlqHeaders.Add("x-original-offset", Encoding.UTF8.GetBytes(result.Offset.Value.ToString()));
            dlqHeaders.Add("x-dead-letter-reason", Encoding.UTF8.GetBytes("max-retries-exhausted"));

            await producer.ProduceAsync(deadLetterTopic, new Message<string, byte[]>
            {
                Key = result.Message.Key,
                Value = result.Message.Value,
                Headers = dlqHeaders
            });

            _logger.LogWarning(
                "Message sent to DLQ [{DeadLetterTopic}] from {Topic}[{Partition}]@{Offset}",
                deadLetterTopic, result.Topic, result.Partition.Value, result.Offset.Value);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex,
                "FAILED to send message to DLQ [{DeadLetterTopic}] from {Topic}[{Partition}]@{Offset}",
                deadLetterTopic, result.Topic, result.Partition.Value, result.Offset.Value);
        }
    }

    private static Dictionary<string, string> ExtractHeaders(Headers? headers)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (headers is null) return result;

        foreach (var header in headers)
        {
            result[header.Key] = Encoding.UTF8.GetString(header.GetValueBytes());
        }
        return result;
    }
}
