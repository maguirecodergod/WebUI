using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Manages the lifecycle of Confluent.Kafka producer and consumer instances.
/// Provides thread-safe, lazy-initialized IProducer and IConsumer builders.
/// </summary>
public sealed class KafkaConnectionFactory : IDisposable
{
    private readonly KafkaOptions _options;
    private readonly ILogger<KafkaConnectionFactory> _logger;

    private IProducer<string, byte[]>? _producer;
    private readonly Lock _producerLock = new();
    private bool _disposed;

    public KafkaConnectionFactory(
        IOptions<KafkaOptions> options,
        ILogger<KafkaConnectionFactory> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gets or creates the shared producer instance (thread-safe, singleton per service).
    /// </summary>
    public IProducer<string, byte[]> GetProducer()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_producer is not null)
            return _producer;

        lock (_producerLock)
        {
            if (_producer is not null)
                return _producer;

            var config = new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                Acks = _options.Producer.Acks,
                EnableIdempotence = _options.Producer.EnableIdempotence,
                MaxInFlight = _options.Producer.MaxInFlight,
                LingerMs = _options.Producer.LingerMs,
                CompressionType = _options.Producer.CompressionType,
                MessageSendMaxRetries = _options.Producer.MessageSendMaxRetries,
                RetryBackoffMs = _options.Producer.RetryBackoffMs,
                RequestTimeoutMs = _options.Producer.RequestTimeoutMs
            };

            _producer = new ProducerBuilder<string, byte[]>(config)
                .SetErrorHandler((_, error) =>
                    _logger.LogError("Kafka producer error: {Error}", error.Reason))
                .SetLogHandler((_, log) =>
                    _logger.LogDebug("Kafka producer log: {Message}", log.Message))
                .Build();

            _logger.LogInformation(
                "Kafka producer created for {Servers}", _options.BootstrapServers);

            return _producer;
        }
    }

    /// <summary>
    /// Creates a new consumer instance for the specified group.
    /// Each consumer group/subscription should have its own consumer.
    /// </summary>
    public IConsumer<string, byte[]> CreateConsumer(string? groupIdOverride = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = groupIdOverride ?? _options.Consumer.GroupId,
            AutoOffsetReset = _options.Consumer.AutoOffsetReset,
            EnableAutoCommit = _options.Consumer.EnableAutoCommit,
            AutoCommitIntervalMs = _options.Consumer.AutoCommitIntervalMs,
            SessionTimeoutMs = _options.Consumer.SessionTimeoutMs,
            HeartbeatIntervalMs = _options.Consumer.HeartbeatIntervalMs,
            MaxPollIntervalMs = _options.Consumer.MaxPollIntervalMs,
            AllowAutoCreateTopics = _options.Consumer.AllowAutoCreateTopics
        };

        var consumer = new ConsumerBuilder<string, byte[]>(config)
            .SetErrorHandler((_, error) =>
                _logger.LogError("Kafka consumer [{GroupId}] error: {Error}",
                    config.GroupId, error.Reason))
            .SetLogHandler((_, log) =>
                _logger.LogDebug("Kafka consumer [{GroupId}] log: {Message}",
                    config.GroupId, log.Message))
            .SetPartitionsAssignedHandler((_, partitions) =>
                _logger.LogInformation(
                    "Kafka consumer [{GroupId}] assigned: {Partitions}",
                    config.GroupId, string.Join(", ", partitions)))
            .SetPartitionsRevokedHandler((_, partitions) =>
                _logger.LogWarning(
                    "Kafka consumer [{GroupId}] revoked: {Partitions}",
                    config.GroupId, string.Join(", ", partitions)))
            .Build();

        _logger.LogInformation(
            "Kafka consumer created for group [{GroupId}]", config.GroupId);

        return consumer;
    }

    /// <summary>
    /// Ensures that a topic exists, creating it if necessary.
    /// </summary>
    public async Task EnsureTopicExistsAsync(
        string topic,
        int numPartitions = 3,
        short replicationFactor = 1,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Consumer.AllowAutoCreateTopics)
            return;

        try
        {
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = _options.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            // Check if topic exists
            var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(5));
            if (metadata.Topics.Any(t => t.Topic == topic))
            {
                _logger.LogDebug("Topic '{Topic}' already exists.", topic);
                return;
            }
        }
        catch
        {
            // Topic doesn't exist or error checking, try to create it
        }

        try
        {
            var adminConfig = new AdminClientConfig
            {
                BootstrapServers = _options.BootstrapServers
            };

            using var adminClient = new AdminClientBuilder(adminConfig).Build();

            var specs = new TopicSpecification[]
            {
                new()
                {
                    Name = topic,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                }
            };

            await adminClient.CreateTopicsAsync(specs);
            _logger.LogInformation(
                "Created Kafka topic '{Topic}' with {Partitions} partitions (replication: {Replication})",
                topic, numPartitions, replicationFactor);

            // Wait a bit for topic to be ready
            await Task.Delay(500, cancellationToken);
        }
        catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
        {
            _logger.LogDebug("Topic '{Topic}' already exists (race condition).", topic);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create topic '{Topic}'. It may need to be created manually.", topic);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _logger.LogInformation("Kafka connection factory disposed");
    }
}
