namespace LHA.EventBus;

/// <summary>
/// Base record for integration events with full distributed tracing metadata.
/// <para>
/// Designed for event-sourcing, multi-region deployment, and billions of messages.
/// Uses V7 UUIDs for time-ordered globally unique identifiers.
/// </para>
/// <example>
/// <code>
/// public sealed record OrderPlaced : IntegrationEvent
/// {
///     public required Guid OrderId { get; init; }
///     public required decimal Amount { get; init; }
/// }
/// </code>
/// </example>
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTimeOffset OccurredAtUtc { get; init; } = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public int Version { get; init; } = 1;

    /// <summary>End-to-end correlation identifier across service boundaries.</summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>The <see cref="EventId"/> of the event that caused this event.</summary>
    public Guid? CausationId { get; init; }

    /// <summary>Tenant identifier for multi-tenant routing and data residency.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>
    /// Logical partition key for ordered processing (e.g., aggregate ID).
    /// Kafka/RabbitMQ transports use this for partition/shard selection.
    /// </summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Data residency region code (e.g., "EU", "US", "APAC").
    /// Used for multi-region deployment and data residency compliance.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>Source service/application that produced this event.</summary>
    public string? Source { get; init; }
}
