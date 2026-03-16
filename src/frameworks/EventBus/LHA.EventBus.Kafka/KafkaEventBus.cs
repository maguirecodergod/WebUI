using System.Text.Json;
using LHA.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.Kafka;

/// <summary>
/// <see cref="IEventBus"/> implementation backed by Apache Kafka via <see cref="IMessagePublisher"/>.
/// <para>
/// Bridges the high-level EventBus abstractions (IntegrationEvent, EventMetadata) to the
/// low-level MessageBroker layer (MessageEnvelope, IMessagePublisher).
/// Supports transactional outbox, inbox deduplication, event versioning,
/// region filtering, and multi-tenant routing.
/// </para>
/// </summary>
internal sealed class KafkaEventBus : IEventBus
{
    private readonly IMessagePublisher _publisher;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventNameResolver _nameResolver;
    private readonly EventBusOptions _options;
    private readonly KafkaEventBusOptions _kafkaOptions;
    private readonly IOutboxStore _outboxStore;
    private readonly ILogger<KafkaEventBus> _logger;

    public KafkaEventBus(
        IMessagePublisher publisher,
        IServiceProvider serviceProvider,
        IEventNameResolver nameResolver,
        IOptions<EventBusOptions> options,
        IOptions<KafkaEventBusOptions> kafkaOptions,
        IOutboxStore outboxStore,
        ILogger<KafkaEventBus>? logger = null)
    {
        _publisher = publisher;
        _serviceProvider = serviceProvider;
        _nameResolver = nameResolver;
        _options = options.Value;
        _kafkaOptions = kafkaOptions.Value;
        _outboxStore = outboxStore;
        _logger = logger ?? NullLogger<KafkaEventBus>.Instance;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
        => PublishAsync(@event, new EventPublishOptions(), cancellationToken);

    public async Task PublishAsync<TEvent>(TEvent @event, EventPublishOptions options, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        var eventName = _nameResolver.GetName(typeof(TEvent));
        var eventVersion = _nameResolver.GetVersion(typeof(TEvent));
        var metadata = BuildMetadata(@event, eventName, eventVersion, options);

        _logger.LogDebug("KafkaEventBus publishing '{EventName}' v{Version}.", eventName, eventVersion);

        // Region filtering
        if (ShouldSkipForRegion(options.TargetRegion))
        {
            _logger.LogDebug("Skipping '{EventName}' — region mismatch.", eventName);
            return;
        }

        // Outbox path
        if (options.UseOutbox && _options.EnableOutbox)
        {
            await PersistToOutboxAsync(@event, metadata, options, cancellationToken);
            return;
        }

        // Direct publish to Kafka
        await PublishToKafkaAsync(@event, metadata, cancellationToken);
    }

    /// <summary>
    /// Publishes a deserialized event directly to Kafka.
    /// Called both from direct publish and from outbox forwarding.
    /// </summary>
    internal async Task PublishToKafkaAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken ct)
        where TEvent : class
    {
        var topic = ResolveTopic(metadata.EventName);

        var envelope = new MessageEnvelope<TEvent>
        {
            MessageId = metadata.EventId.ToString("N"),
            Payload = @event,
            TenantId = metadata.TenantId?.ToString(),
            CorrelationId = metadata.CorrelationId?.ToString(),
            CausationId = metadata.CausationId?.ToString(),
            Source = metadata.Source,
            SchemaVersion = metadata.EventVersion.ToString(),
            PartitionKey = metadata.PartitionKey,
            Timestamp = metadata.OccurredAtUtc,
            Metadata = new Dictionary<string, string>
            {
                ["x-event-name"] = metadata.EventName,
                ["x-event-version"] = metadata.EventVersion.ToString(),
            }
        };

        await _publisher.PublishAsync(topic, envelope, ct);

        _logger.LogInformation("Published '{EventName}' to Kafka topic '{Topic}'.", metadata.EventName, topic);
    }

    private string ResolveTopic(string eventName)
    {
        if (_kafkaOptions.EventTopicMappings.TryGetValue(eventName, out var topic))
            return topic;
        return _kafkaOptions.DefaultTopic ?? eventName.Replace('.', '-').ToLowerInvariant();
    }

    private async Task PersistToOutboxAsync<TEvent>(
        TEvent @event, EventMetadata metadata, EventPublishOptions options, CancellationToken ct)
        where TEvent : class
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(@event);

        await _outboxStore.SaveAsync(new OutboxMessageInfo
        {
            Id = metadata.EventId,
            EventName = metadata.EventName,
            EventVersion = metadata.EventVersion,
            Payload = payload,
            Metadata = metadata,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            PartitionKey = options.PartitionKey ?? metadata.PartitionKey
        }, ct);

        _logger.LogDebug("Event '{EventName}' persisted to outbox for Kafka delivery.", metadata.EventName);
    }

    private EventMetadata BuildMetadata<TEvent>(TEvent @event, string eventName, int eventVersion, EventPublishOptions options)
    {
        var ie = @event as IntegrationEvent;
        return new EventMetadata
        {
            EventName = eventName,
            EventVersion = eventVersion,
            EventId = ie?.EventId ?? Guid.CreateVersion7(),
            OccurredAtUtc = ie?.OccurredAtUtc ?? DateTimeOffset.UtcNow,
            CorrelationId = options.CorrelationId ?? ie?.CorrelationId,
            CausationId = options.CausationId ?? ie?.CausationId,
            TenantId = ie?.TenantId,
            PartitionKey = options.PartitionKey ?? ie?.PartitionKey,
            Region = options.TargetRegion ?? ie?.Region ?? _options.CurrentRegion,
            Source = ie?.Source ?? _options.ApplicationName,
            ConsumerGroup = _options.ConsumerGroup
        };
    }

    private bool ShouldSkipForRegion(string? targetRegion)
    {
        if (!_options.EnforceRegionFiltering || string.IsNullOrWhiteSpace(_options.CurrentRegion))
            return false;

        var region = targetRegion ?? _options.CurrentRegion;
        return !string.IsNullOrWhiteSpace(region)
            && !string.Equals(region, _options.CurrentRegion, StringComparison.OrdinalIgnoreCase);
    }
}
