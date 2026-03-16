namespace LHA.Ddd.Domain;

/// <summary>
/// Interface for entities (typically aggregate roots) that can raise domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Domain events raised by this entity, pending dispatch.
    /// </summary>
    IReadOnlyCollection<DomainEventRecord> DomainEvents { get; }

    /// <summary>
    /// Clears all pending domain events. Called after events have been dispatched.
    /// </summary>
    void ClearDomainEvents();
}
