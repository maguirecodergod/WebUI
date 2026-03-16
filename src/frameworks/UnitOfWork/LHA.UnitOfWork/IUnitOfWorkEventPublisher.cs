namespace LHA.UnitOfWork;

/// <summary>
/// Publishes domain events collected during a UoW after successful completion.
/// <para>
/// Implementations bridge to the application's event bus (e.g. MediatR for local,
/// message broker for distributed).
/// </para>
/// </summary>
public interface IUnitOfWorkEventPublisher
{
    /// <summary>Publishes in-process domain events (e.g. via MediatR / in-memory bus).</summary>
    Task PublishLocalEventsAsync(
        IReadOnlyList<UnitOfWorkEventRecord> localEvents,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes distributed domain events (e.g. via outbox pattern / message broker).
    /// </summary>
    Task PublishDistributedEventsAsync(
        IReadOnlyList<UnitOfWorkEventRecord> distributedEvents,
        CancellationToken cancellationToken = default);
}
