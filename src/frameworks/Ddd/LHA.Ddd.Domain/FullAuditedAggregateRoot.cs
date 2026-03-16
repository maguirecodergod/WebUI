using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Aggregate root base class that tracks full audit information including soft-delete.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class FullAuditedAggregateRoot<TKey> : AuditedAggregateRoot<TKey>, IFullAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletionTime { get; set; }

    /// <inheritdoc />
    public Guid? DeleterId { get; set; }
}
