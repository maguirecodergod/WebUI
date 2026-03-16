using Confluent.Kafka;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Configuration options for the LHA Kafka MessageBroker.
/// Configures cluster connection, producer, consumer, and tenant strategies.
/// </summary>
public sealed class KafkaOptions
{
    /// <summary>Kafka bootstrap servers (e.g. "localhost:9092,broker2:9092").</summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>Global default tenant topic strategy.</summary>
    public TenantTopicStrategy DefaultTenantStrategy { get; set; } = TenantTopicStrategy.TenantPartition;

    /// <summary>Prefix for tenant-specific topics (used with TenantPrefixTopic strategy).</summary>
    public string TenantTopicPrefix { get; set; } = "tenant";

    /// <summary>Producer configuration.</summary>
    public KafkaProducerOptions Producer { get; set; } = new();

    /// <summary>Consumer configuration.</summary>
    public KafkaConsumerOptions Consumer { get; set; } = new();

    /// <summary>Per-topic overrides.</summary>
    public IDictionary<string, KafkaTopicOptions> Topics { get; set; } = new Dictionary<string, KafkaTopicOptions>();

    /// <summary>
    /// Gets or creates topic-specific options.
    /// </summary>
    public KafkaTopicOptions GetTopicOptions(string topic)
    {
        if (!Topics.TryGetValue(topic, out var options))
        {
            options = new KafkaTopicOptions();
            Topics[topic] = options;
        }
        return options;
    }
}

/// <summary>
/// Kafka producer-specific configuration.
/// </summary>
public sealed class KafkaProducerOptions
{
    /// <summary>Acknowledgement level: 0=fire-and-forget, 1=leader, -1=all ISR.</summary>
    public Acks Acks { get; set; } = Acks.All;

    /// <summary>Enable idempotent producer for exactly-once semantics.</summary>
    public bool EnableIdempotence { get; set; } = true;

    /// <summary>Max in-flight requests per connection (set to 5 with idempotence).</summary>
    public int MaxInFlight { get; set; } = 5;

    /// <summary>Linger time in milliseconds for batching.</summary>
    public int LingerMs { get; set; } = 5;

    /// <summary>Compression type for message batches.</summary>
    public CompressionType CompressionType { get; set; } = CompressionType.Lz4;

    /// <summary>Max retries for transient failures.</summary>
    public int MessageSendMaxRetries { get; set; } = 3;

    /// <summary>Retry backoff in milliseconds.</summary>
    public int RetryBackoffMs { get; set; } = 100;

    /// <summary>Request timeout in milliseconds.</summary>
    public int RequestTimeoutMs { get; set; } = 30000;
}

/// <summary>
/// Kafka consumer-specific configuration.
/// </summary>
public sealed class KafkaConsumerOptions
{
    /// <summary>Consumer group ID.</summary>
    public string GroupId { get; set; } = "lha-default-group";

    /// <summary>Auto offset reset policy.</summary>
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

    /// <summary>Enable auto commit.</summary>
    public bool EnableAutoCommit { get; set; } = false;

    /// <summary>Auto commit interval in milliseconds (if auto commit is enabled).</summary>
    public int AutoCommitIntervalMs { get; set; } = 5000;

    /// <summary>Max poll records per batch.</summary>
    public int MaxPollRecords { get; set; } = 500;

    /// <summary>Session timeout in milliseconds.</summary>
    public int SessionTimeoutMs { get; set; } = 45000;

    /// <summary>Heartbeat interval in milliseconds.</summary>
    public int HeartbeatIntervalMs { get; set; } = 3000;

    /// <summary>Max poll interval in milliseconds.</summary>
    public int MaxPollIntervalMs { get; set; } = 300000;

    /// <summary>Number of concurrent consumer workers per subscription.</summary>
    public int ConcurrencyLevel { get; set; } = 1;
}

/// <summary>
/// Per-topic configuration overrides.
/// </summary>
public sealed class KafkaTopicOptions
{
    /// <summary>Override tenant strategy for this specific topic.</summary>
    public TenantTopicStrategy? TenantStrategy { get; set; }

    /// <summary>Override consumer group for this topic.</summary>
    public string? GroupId { get; set; }

    /// <summary>Dead-letter topic for failed messages.</summary>
    public string? DeadLetterTopic { get; set; }

    /// <summary>Max retry attempts before sending to dead-letter.</summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>Retry backoff base in milliseconds (exponential).</summary>
    public int RetryBackoffBaseMs { get; set; } = 1000;
}
