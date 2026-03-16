using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Aggregate root base class that tracks creation and modification audit information.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class AuditedAggregateRoot<TKey> : CreationAuditedAggregateRoot<TKey>, IAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset? LastModificationTime { get; set; }

    /// <inheritdoc />
    public Guid? LastModifierId { get; set; }
}
