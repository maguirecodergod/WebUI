using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Entity base class that tracks creation and modification audit information.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class AuditedEntity<TKey> : CreationAuditedEntity<TKey>, IAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset? LastModificationTime { get; set; }

    /// <inheritdoc />
    public Guid? LastModifierId { get; set; }
}
