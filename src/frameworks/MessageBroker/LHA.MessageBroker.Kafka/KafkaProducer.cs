using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Kafka producer implementing the broker-agnostic <see cref="IMessagePublisher"/> interface.
/// Routes messages to the correct topic/partition based on tenant strategy.
/// Thread-safe — uses a shared underlying Confluent producer.
/// </summary>
public sealed class KafkaProducer : IMessagePublisher
{
    private readonly KafkaConnectionFactory _connectionFactory;
    private readonly IMessageSerializer _serializer;
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(
        KafkaConnectionFactory connectionFactory,
        IMessageSerializer serializer,
        IOptions<KafkaOptions> options,
        ILogger<KafkaProducer> logger)
    {
        _connectionFactory = connectionFactory;
        _serializer = serializer;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PublishResult> PublishAsync<T>(
        string destination,
        MessageEnvelope<T> envelope,
        CancellationToken cancellationToken = default) where T : class
    {
        var resolvedTopic = ResolveTopic(destination, envelope.TenantId);

        // Ensure topic exists before publishing
        if (_options.Consumer.AllowAutoCreateTopics)
        {
            await _connectionFactory.EnsureTopicExistsAsync(
                resolvedTopic,
                cancellationToken: cancellationToken);
        }

        var partitionKey = ResolvePartitionKey(destination, envelope);
        var message = BuildKafkaMessage(envelope);

        var producer = _connectionFactory.GetProducer();

        _logger.LogDebug(
            "Publishing to {Topic} | Key={Key} | TenantId={TenantId} | MessageId={MessageId}",
            resolvedTopic, partitionKey, envelope.TenantId, envelope.MessageId);

        var result = await producer.ProduceAsync(
            resolvedTopic,
            new Message<string, byte[]>
            {
                Key = partitionKey,
                Value = message.Value,
                Headers = message.Headers
            },
            cancellationToken);

        _logger.LogInformation(
            "Published to {Topic}[{Partition}]@{Offset} | TenantId={TenantId} | MessageId={MessageId}",
            result.Topic, result.Partition.Value, result.Offset.Value,
            envelope.TenantId, envelope.MessageId);

        return new PublishResult
        {
            Destination = result.Topic,
            MessageId = envelope.MessageId,
            Timestamp = result.Timestamp.Type != TimestampType.NotAvailable
                ? DateTimeOffset.FromUnixTimeMilliseconds(result.Timestamp.UnixTimestampMs)
                : null,
            BrokerMetadata = new Dictionary<string, object>
            {
                [KafkaMetadataKeys.Topic] = result.Topic,
                [KafkaMetadataKeys.Partition] = result.Partition.Value,
                [KafkaMetadataKeys.Offset] = result.Offset.Value
            }
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PublishResult>> PublishBatchAsync<T>(
        string destination,
        IEnumerable<MessageEnvelope<T>> envelopes,
        CancellationToken cancellationToken = default) where T : class
    {
        var tasks = new List<Task<DeliveryResult<string, byte[]>>>();
        var messageIds = new List<string>();

        // Ensure topic exists before publishing batch
        if (_options.Consumer.AllowAutoCreateTopics)
        {
            await _connectionFactory.EnsureTopicExistsAsync(
                destination,
                cancellationToken: cancellationToken);
        }

        var producer = _connectionFactory.GetProducer();

        foreach (var envelope in envelopes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var resolvedTopic = ResolveTopic(destination, envelope.TenantId);
            var partitionKey = ResolvePartitionKey(destination, envelope);
            var message = BuildKafkaMessage(envelope);

            var task = producer.ProduceAsync(
                resolvedTopic,
                new Message<string, byte[]>
                {
                    Key = partitionKey,
                    Value = message.Value,
                    Headers = message.Headers
                },
                cancellationToken);

            tasks.Add(task);
            messageIds.Add(envelope.MessageId);
        }

        var deliveryResults = await Task.WhenAll(tasks);
        var results = new List<PublishResult>(deliveryResults.Length);

        for (var i = 0; i < deliveryResults.Length; i++)
        {
            var result = deliveryResults[i];
            results.Add(new PublishResult
            {
                Destination = result.Topic,
                MessageId = messageIds[i],
                Timestamp = result.Timestamp.Type != TimestampType.NotAvailable
                    ? DateTimeOffset.FromUnixTimeMilliseconds(result.Timestamp.UnixTimestampMs)
                    : null,
                BrokerMetadata = new Dictionary<string, object>
                {
                    [KafkaMetadataKeys.Topic] = result.Topic,
                    [KafkaMetadataKeys.Partition] = result.Partition.Value,
                    [KafkaMetadataKeys.Offset] = result.Offset.Value
                }
            });
        }

        _logger.LogInformation("Published batch of {Count} messages to {Topic}", results.Count, destination);
        return results;
    }

    private string ResolveTopic(string baseTopic, string? tenantId)
    {
        var strategy = GetTenantStrategy(baseTopic);

        return strategy switch
        {
            TenantTopicStrategy.TenantPrefixTopic when !string.IsNullOrEmpty(tenantId)
                => $"{_options.TenantTopicPrefix}.{tenantId}.{baseTopic}",
            _ => baseTopic
        };
    }

    private string ResolvePartitionKey<T>(string baseTopic, MessageEnvelope<T> envelope) where T : class
    {
        var strategy = GetTenantStrategy(baseTopic);

        return strategy switch
        {
            TenantTopicStrategy.TenantPartition when !string.IsNullOrEmpty(envelope.TenantId)
                => envelope.TenantId,
            _ => envelope.PartitionKey ?? envelope.MessageId
        };
    }

    private TenantTopicStrategy GetTenantStrategy(string topic)
    {
        if (_options.Topics.TryGetValue(topic, out var topicOptions) && topicOptions.TenantStrategy.HasValue)
            return topicOptions.TenantStrategy.Value;
        return _options.DefaultTenantStrategy;
    }

    private Message<string, byte[]> BuildKafkaMessage<T>(MessageEnvelope<T> envelope) where T : class
    {
        // When Payload is already byte[] (e.g. from outbox processor), pass through directly
        // to avoid double-serialization (byte[] → base64 JSON).
        var payload = envelope.Payload is byte[] raw
            ? raw
            : _serializer.Serialize(envelope.Payload);
        var headers = new Headers();

        AddHeader(headers, MessageHeaders.MessageType, typeof(T).AssemblyQualifiedName!);
        AddHeader(headers, MessageHeaders.Timestamp, envelope.Timestamp.ToString("O"));
        AddHeader(headers, MessageHeaders.SchemaVersion, envelope.SchemaVersion);

        if (!string.IsNullOrEmpty(envelope.TenantId))
            AddHeader(headers, MessageHeaders.TenantId, envelope.TenantId);
        if (!string.IsNullOrEmpty(envelope.CorrelationId))
            AddHeader(headers, MessageHeaders.CorrelationId, envelope.CorrelationId);
        if (!string.IsNullOrEmpty(envelope.CausationId))
            AddHeader(headers, MessageHeaders.CausationId, envelope.CausationId);
        if (!string.IsNullOrEmpty(envelope.UserId))
            AddHeader(headers, MessageHeaders.UserId, envelope.UserId);
        if (!string.IsNullOrEmpty(envelope.Source))
            AddHeader(headers, MessageHeaders.Source, envelope.Source);

        foreach (var kvp in envelope.Metadata)
            AddHeader(headers, kvp.Key, kvp.Value);

        return new Message<string, byte[]>
        {
            Key = envelope.MessageId,
            Value = payload,
            Headers = headers
        };
    }

    private static void AddHeader(Headers headers, string key, string value)
    {
        headers.Add(key, Encoding.UTF8.GetBytes(value));
    }
}
