using LHA.Ddd.Domain;
using LHA.EventBus;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// EF Core persistence entity for the idempotent inbox pattern.
/// Inherits <see cref="Entity{TKey}"/> from the DDD building blocks.
/// <para>
/// Uses a surrogate <see cref="Entity{TKey}.Id"/> as primary key, with a unique
/// composite index on (<see cref="EventId"/>, <see cref="ConsumerGroup"/>) for deduplication.
/// Maps to/from <see cref="InboxMessageInfo"/> at the persistence boundary.
/// </para>
/// </summary>
public sealed class InboxMessage : Entity<Guid>
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

    /// <summary>
    /// Creates an <see cref="InboxMessage"/> entity from an <see cref="InboxMessageInfo"/> DTO.
    /// </summary>
    public static InboxMessage FromInfo(InboxMessageInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        return new InboxMessage
        {
            Id = Guid.CreateVersion7(),
            EventId = info.EventId,
            EventName = info.EventName,
            ConsumerGroup = info.ConsumerGroup,
            ReceivedAtUtc = info.ReceivedAtUtc,
            ProcessedAtUtc = info.ProcessedAtUtc
        };
    }

    /// <summary>
    /// Converts this persistence entity back to a transport-agnostic DTO.
    /// </summary>
    public InboxMessageInfo ToInfo() => new()
    {
        EventId = EventId,
        EventName = EventName,
        ConsumerGroup = ConsumerGroup,
        ReceivedAtUtc = ReceivedAtUtc,
        ProcessedAtUtc = ProcessedAtUtc
    };
}
