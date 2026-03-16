using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.EventBus;

/// <summary>
/// In-process <see cref="IEventBus"/> implementation that dispatches events to
/// DI-resolved <see cref="IEventHandler{TEvent}"/> instances within the same process.
/// <para>
/// Suitable for monolith deployments, integration testing, and development.
/// For cross-service communication, use a transport like Kafka or RabbitMQ.
/// </para>
/// </summary>
internal sealed class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventNameResolver _nameResolver;
    private readonly EventBusOptions _options;
    private readonly IOutboxStore _outboxStore;
    private readonly ILogger<InMemoryEventBus> _logger;

    // Cache handler types per event type to avoid repeated reflection
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();

    public InMemoryEventBus(
        IServiceProvider serviceProvider,
        IEventNameResolver nameResolver,
        IOptions<EventBusOptions> options,
        IOutboxStore outboxStore,
        ILogger<InMemoryEventBus>? logger = null)
    {
        _serviceProvider = serviceProvider;
        _nameResolver = nameResolver;
        _options = options.Value;
        _outboxStore = outboxStore;
        _logger = logger ?? NullLogger<InMemoryEventBus>.Instance;
    }

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        return PublishAsync(@event, new EventPublishOptions(), cancellationToken);
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent @event, EventPublishOptions options, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(@event);

        var eventName = _nameResolver.GetName(typeof(TEvent));
        var eventVersion = _nameResolver.GetVersion(typeof(TEvent));

        _logger.LogDebug("Publishing event '{EventName}' v{Version}.", eventName, eventVersion);

        // Build metadata
        var metadata = BuildMetadata(@event, eventName, eventVersion, options);

        // Region filtering: skip if event targets a different region
        if (ShouldSkipForRegion(options.TargetRegion))
        {
            _logger.LogDebug("Skipping event '{EventName}' — target region '{Region}' does not match current region '{CurrentRegion}'.",
                eventName, options.TargetRegion, _options.CurrentRegion);
            return;
        }

        // Outbox path: persist for reliable delivery
        if (options.UseOutbox && _options.EnableOutbox)
        {
            await PersistToOutboxAsync(@event, metadata, options, cancellationToken);
            return;
        }

        // Direct in-process dispatch
        await DispatchAsync(@event, metadata, cancellationToken);
    }

    private async Task DispatchAsync<TEvent>(TEvent @event, EventMetadata metadata, CancellationToken cancellationToken)
        where TEvent : class
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        var context = new EventContext
        {
            Metadata = metadata,
            ServiceProvider = scope.ServiceProvider,
            CancellationToken = cancellationToken
        };

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(@event, context, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Handler {HandlerType} failed for event '{EventName}'.",
                    handler.GetType().Name, metadata.EventName);
                throw;
            }
        }
    }

    private async Task PersistToOutboxAsync<TEvent>(
        TEvent @event, EventMetadata metadata, EventPublishOptions options, CancellationToken cancellationToken)
        where TEvent : class
    {
        var payload = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(@event);

        var outboxMessage = new OutboxMessageInfo
        {
            Id = metadata.EventId,
            EventName = metadata.EventName,
            EventVersion = metadata.EventVersion,
            Payload = payload,
            Metadata = metadata,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            PartitionKey = options.PartitionKey ?? metadata.PartitionKey
        };

        await _outboxStore.SaveAsync(outboxMessage, cancellationToken);
        _logger.LogDebug("Event '{EventName}' persisted to outbox.", metadata.EventName);
    }

    private EventMetadata BuildMetadata<TEvent>(TEvent @event, string eventName, int eventVersion, EventPublishOptions options)
    {
        var integrationEvent = @event as IntegrationEvent;

        return new EventMetadata
        {
            EventName = eventName,
            EventVersion = eventVersion,
            EventId = integrationEvent?.EventId ?? Guid.CreateVersion7(),
            OccurredAtUtc = integrationEvent?.OccurredAtUtc ?? DateTimeOffset.UtcNow,
            CorrelationId = options.CorrelationId ?? integrationEvent?.CorrelationId,
            CausationId = options.CausationId ?? integrationEvent?.CausationId,
            TenantId = integrationEvent?.TenantId,
            PartitionKey = options.PartitionKey ?? integrationEvent?.PartitionKey,
            Region = options.TargetRegion ?? integrationEvent?.Region ?? _options.CurrentRegion,
            Source = integrationEvent?.Source ?? _options.ApplicationName,
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
