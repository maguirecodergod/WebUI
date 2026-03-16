namespace LHA.EventBus;

/// <summary>
/// Wire-format metadata attached to every event during transport.
/// Separated from the business payload for clean serialization boundaries.
/// </summary>
public sealed record EventMetadata
{
    /// <summary>Fully-qualified event name (e.g., "OrderService.OrderPlaced").</summary>
    public required string EventName { get; init; }

    /// <summary>Schema version for backward-compatible event versioning.</summary>
    public required int EventVersion { get; init; }

    /// <summary>Globally unique event identifier.</summary>
    public Guid EventId { get; init; }

    /// <summary>UTC instant when the event occurred.</summary>
    public DateTimeOffset OccurredAtUtc { get; init; }

    /// <summary>End-to-end correlation identifier.</summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>The event ID that caused this event.</summary>
    public Guid? CausationId { get; init; }

    /// <summary>Tenant identifier for multi-tenant routing.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>Logical partition key for ordered processing.</summary>
    public string? PartitionKey { get; init; }

    /// <summary>Data residency region code.</summary>
    public string? Region { get; init; }

    /// <summary>Source service name.</summary>
    public string? Source { get; init; }

    /// <summary>Consumer group that should process this event.</summary>
    public string? ConsumerGroup { get; init; }

    /// <summary>
    /// Creates <see cref="EventMetadata"/> from an <see cref="IntegrationEvent"/>.
    /// </summary>
    public static EventMetadata FromEvent(IntegrationEvent @event, string eventName)
    {
        ArgumentNullException.ThrowIfNull(@event);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);

        return new EventMetadata
        {
            EventName = eventName,
            EventVersion = @event.Version,
            EventId = @event.EventId,
            OccurredAtUtc = @event.OccurredAtUtc,
            CorrelationId = @event.CorrelationId,
            CausationId = @event.CausationId,
            TenantId = @event.TenantId,
            PartitionKey = @event.PartitionKey,
            Region = @event.Region,
            Source = @event.Source
        };
    }
}
