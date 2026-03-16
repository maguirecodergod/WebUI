using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Aggregate root base class that tracks creation audit information.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class CreationAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc />
    public Guid? CreatorId { get; set; }
}
