using System.Text.Json;
using LHA.Ddd.Domain;
using LHA.EventBus;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// EF Core persistence entity for the transactional outbox pattern.
/// Inherits <see cref="Entity{TKey}"/> from the DDD building blocks.
/// <para>
/// Maps to/from <see cref="OutboxMessageInfo"/> at the persistence boundary
/// via <see cref="FromInfo"/> and <see cref="ToInfo"/>.
/// </para>
/// </summary>
public sealed class OutboxMessage : Entity<Guid>
{
    /// <summary>Canonical event name for deserialization routing.</summary>
    public required string EventName { get; init; }

    /// <summary>Schema version of the event.</summary>
    public int EventVersion { get; init; } = 1;

    /// <summary>Serialized event payload (JSON/binary).</summary>
    public required byte[] Payload { get; init; }

    /// <summary>JSON-serialized <see cref="EventMetadata"/> for transport headers.</summary>
    public string? MetadataJson { get; init; }

    /// <summary>UTC time the message was written to the outbox.</summary>
    public required DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>UTC time the message was forwarded to the transport (<c>null</c> = pending).</summary>
    public DateTimeOffset? ProcessedAtUtc { get; set; }

    /// <summary>Number of failed delivery attempts.</summary>
    public int RetryCount { get; set; }

    /// <summary>Partition key for ordered delivery.</summary>
    public string? PartitionKey { get; init; }

    /// <summary>
    /// Creates an <see cref="OutboxMessage"/> entity from an <see cref="OutboxMessageInfo"/> DTO.
    /// </summary>
    public static OutboxMessage FromInfo(OutboxMessageInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);
        return new OutboxMessage
        {
            Id = info.Id,
            EventName = info.EventName,
            EventVersion = info.EventVersion,
            Payload = info.Payload,
            MetadataJson = info.Metadata is not null
                ? JsonSerializer.Serialize(info.Metadata)
                : null,
            CreatedAtUtc = info.CreatedAtUtc,
            ProcessedAtUtc = info.ProcessedAtUtc,
            RetryCount = info.RetryCount,
            PartitionKey = info.PartitionKey
        };
    }

    /// <summary>
    /// Converts this persistence entity back to a transport-agnostic DTO.
    /// </summary>
    public OutboxMessageInfo ToInfo() => new()
    {
        Id = Id,
        EventName = EventName,
        EventVersion = EventVersion,
        Payload = Payload,
        Metadata = MetadataJson is not null
            ? JsonSerializer.Deserialize<EventMetadata>(MetadataJson)
            : null,
        CreatedAtUtc = CreatedAtUtc,
        ProcessedAtUtc = ProcessedAtUtc,
        RetryCount = RetryCount,
        PartitionKey = PartitionKey
    };
}
