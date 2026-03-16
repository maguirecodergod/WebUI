using System.Text.Json;
using LHA.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.RabbitMQ;

/// <summary>
/// <see cref="IEventBus"/> implementation backed by RabbitMQ via <see cref="IMessagePublisher"/>.
/// <para>
/// Bridges the high-level EventBus abstractions (IntegrationEvent, EventMetadata) to the
/// low-level MessageBroker layer (MessageEnvelope, IMessagePublisher).
/// Supports transactional outbox, inbox deduplication, event versioning,
/// region filtering, and multi-tenant routing.
/// </para>
/// </summary>
internal sealed class RabbitMqEventBus : IEventBus
{
    private readonly IMessagePublisher _publisher;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventNameResolver _nameResolver;
    private readonly EventBusOptions _options;
    private readonly RabbitMqEventBusOptions _rmqOptions;
    private readonly IOutboxStore _outboxStore;
    private readonly ILogger<RabbitMqEventBus> _logger;

    public RabbitMqEventBus(
        IMessagePublisher publisher,
        IServiceProvider serviceProvider,
        IEventNameResolver nameResolver,
        IOptions<EventBusOptions> options,
        IOptions<RabbitMqEventBusOptions> rmqOptions,
        IOutboxStore outboxStore,
        ILogger<RabbitMqEventBus>? logger = null)
    {
        _publisher = publisher;
        _serviceProvider = serviceProvider;
        _nameResolver = nameResolver;
        _options = options.Value;
        _rmqOptions = rmqOptions.Value;
        _outboxStore = outboxStore;
        _logger = logger ?? NullLogger<RabbitMqEventBus>.Instance;
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

        _logger.LogDebug("RabbitMqEventBus publishing '{EventName}' v{Version}.", eventName, eventVersion);

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

        // Direct publish to RabbitMQ
        await PublishToRabbitMqAsync(@event, metadata, cancellationToken);
    }

    /// <summary>
    /// Publishes a deserialized event directly to RabbitMQ.
    /// Called both from direct publish and from outbox forwarding.
    /// </summary>
    internal async Task PublishToRabbitMqAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken ct)
        where TEvent : class
    {
        var exchange = ResolveExchange(metadata.EventName);
        var routingKey = ResolveRoutingKey(metadata);

        var envelope = new MessageEnvelope<TEvent>
        {
            MessageId = metadata.EventId.ToString("N"),
            Payload = @event,
            TenantId = metadata.TenantId?.ToString(),
            CorrelationId = metadata.CorrelationId?.ToString(),
            CausationId = metadata.CausationId?.ToString(),
            Source = metadata.Source,
            SchemaVersion = metadata.EventVersion.ToString(),
            RoutingKey = routingKey,
            Timestamp = metadata.OccurredAtUtc,
            Metadata = new Dictionary<string, string>
            {
                ["x-event-name"] = metadata.EventName,
                ["x-event-version"] = metadata.EventVersion.ToString(),
            }
        };

        await _publisher.PublishAsync(exchange, envelope, ct);

        _logger.LogInformation("Published '{EventName}' to RabbitMQ exchange '{Exchange}' routing '{RoutingKey}'.",
            metadata.EventName, exchange, routingKey);
    }

    private string ResolveExchange(string eventName)
    {
        if (_rmqOptions.EventExchangeMappings.TryGetValue(eventName, out var exchange))
            return exchange;
        return _rmqOptions.DefaultExchange ?? "lha.events";
    }

    private string ResolveRoutingKey(EventMetadata metadata)
    {
        if (_rmqOptions.EventRoutingKeyMappings.TryGetValue(metadata.EventName, out var rk))
            return rk;
        return metadata.EventName.Replace('.', '-').ToLowerInvariant();
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

        _logger.LogDebug("Event '{EventName}' persisted to outbox for RabbitMQ delivery.", metadata.EventName);
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
