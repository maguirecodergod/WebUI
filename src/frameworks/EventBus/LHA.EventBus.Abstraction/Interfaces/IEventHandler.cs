namespace LHA.EventBus;

/// <summary>
/// Handles events of type <typeparamref name="TEvent"/>.
/// <para>
/// Implementations are resolved from DI per-scope. A single event type
/// may have multiple handlers executing concurrently.
/// </para>
/// </summary>
/// <typeparam name="TEvent">The event type to handle.</typeparam>
public interface IEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// Handles the event with access to transport metadata via <paramref name="context"/>.
    /// </summary>
    /// <param name="event">The deserialized event payload.</param>
    /// <param name="context">Event context with metadata and scoped service provider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task HandleAsync(TEvent @event, EventContext context, CancellationToken cancellationToken = default);
}
