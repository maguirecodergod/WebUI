namespace LHA.EventBus;

/// <summary>
/// Unified event bus for publishing integration events.
/// <para>
/// Implementations may deliver events in-process (InMemory), via outbox pattern,
/// or directly to a message broker (Kafka, RabbitMQ).
/// </para>
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all registered handlers and/or the configured transport.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;

    /// <summary>
    /// Publishes an event with explicit publish options.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="options">Options controlling routing, outbox usage, and correlation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(TEvent @event, EventPublishOptions options, CancellationToken cancellationToken = default)
        where TEvent : class;
}
