using LHA.Auditing;

namespace LHA.Ddd.Domain;

/// <summary>
/// Base class for aggregate roots. Supports domain events and optimistic concurrency via
/// <see cref="IHasEntityVersion"/> and <see cref="IHasConcurrencyStamp"/>.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IHasDomainEvents, IHasEntityVersion, IHasConcurrencyStamp
    where TKey : notnull
{
    private readonly List<DomainEventRecord> _domainEvents = [];

    /// <inheritdoc />
    public IReadOnlyCollection<DomainEventRecord> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public int EntityVersion { get; set; }

    /// <inheritdoc />
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Raises a domain event on this aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(new DomainEventRecord(domainEvent, TimeProvider.System.GetUtcNow()));
    }

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();
}
