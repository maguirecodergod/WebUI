using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Entity base class that tracks creation audit information.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class CreationAuditedEntity<TKey> : Entity<TKey>, ICreationAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc />
    public Guid? CreatorId { get; set; }
}
