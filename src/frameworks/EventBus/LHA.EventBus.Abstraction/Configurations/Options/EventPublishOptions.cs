namespace LHA.EventBus;

/// <summary>
/// Options for controlling event publishing behavior.
/// </summary>
public sealed class EventPublishOptions
{
    /// <summary>
    /// Correlation identifier propagated across service boundaries.
    /// Overrides <see cref="IntegrationEvent.CorrelationId"/> if set.
    /// </summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>
    /// Causation identifier linking this publish to a source event.
    /// Overrides <see cref="IntegrationEvent.CausationId"/> if set.
    /// </summary>
    public Guid? CausationId { get; init; }

    /// <summary>
    /// When <c>true</c>, the event is persisted to the outbox for reliable delivery
    /// instead of being sent directly to the transport.
    /// Enables distributed transaction avoidance (transactional outbox pattern).
    /// </summary>
    public bool UseOutbox { get; init; }

    /// <summary>
    /// Explicit partition key for ordered delivery.
    /// Overrides <see cref="IntegrationEvent.PartitionKey"/> if set.
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Target region for data residency compliance.
    /// When set, the event is only delivered to consumers in this region.
    /// </summary>
    public string? TargetRegion { get; init; }
}
