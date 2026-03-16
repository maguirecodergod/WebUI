namespace LHA.EventBus;

/// <summary>
/// Represents an event persisted to the transactional outbox.
/// <para>
/// The outbox pattern ensures atomicity between business state changes and event
/// publishing without distributed transactions. Events are written to the outbox
/// in the same database transaction as business data, then forwarded to the
/// message broker by a background processor.
/// </para>
/// </summary>
public sealed class OutboxMessageInfo
{
    /// <summary>Unique message identifier (typically matches <see cref="EventMetadata.EventId"/>).</summary>
    public required Guid Id { get; init; }

    /// <summary>Canonical event name for deserialization routing.</summary>
    public required string EventName { get; init; }

    /// <summary>Schema version of the event.</summary>
    public int EventVersion { get; init; } = 1;

    /// <summary>Serialized event payload (JSON/binary).</summary>
    public required byte[] Payload { get; init; }

    /// <summary>Full event metadata for transport headers.</summary>
    public EventMetadata? Metadata { get; init; }

    /// <summary>UTC time the message was written to the outbox.</summary>
    public required DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>UTC time the message was forwarded to the transport (<c>null</c> = pending).</summary>
    public DateTimeOffset? ProcessedAtUtc { get; set; }

    /// <summary>Number of failed delivery attempts.</summary>
    public int RetryCount { get; set; }

    /// <summary>Partition key for ordered delivery.</summary>
    public string? PartitionKey { get; init; }
}