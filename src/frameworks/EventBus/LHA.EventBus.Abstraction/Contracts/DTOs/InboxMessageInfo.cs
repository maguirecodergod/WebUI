namespace LHA.EventBus;

/// <summary>
/// Represents an event received and tracked in the inbox for idempotent processing.
/// <para>
/// The inbox pattern prevents duplicate event handling in at-least-once delivery
/// environments. Before processing, the consumer checks whether the event has
/// already been handled by this consumer group.
/// </para>
/// </summary>
public sealed class InboxMessageInfo
{
    /// <summary>The event identifier (matches the source event's ID).</summary>
    public required Guid EventId { get; init; }

    /// <summary>Canonical event name.</summary>
    public required string EventName { get; init; }

    /// <summary>Consumer group that processed this event.</summary>
    public required string ConsumerGroup { get; init; }

    /// <summary>UTC time the event was first received.</summary>
    public required DateTimeOffset ReceivedAtUtc { get; init; }

    /// <summary>UTC time processing completed (<c>null</c> = in-progress or failed).</summary>
    public DateTimeOffset? ProcessedAtUtc { get; set; }
}
