using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Entity base class that tracks full audit information including soft-delete.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class FullAuditedEntity<TKey> : AuditedEntity<TKey>, IFullAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletionTime { get; set; }

    /// <inheritdoc />
    public Guid? DeleterId { get; set; }
}
