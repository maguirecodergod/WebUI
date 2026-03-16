namespace LHA.EventBus;

/// <summary>
/// Context passed to <see cref="IEventHandler{TEvent}"/> during event processing.
/// Provides access to metadata, services, and cancellation.
/// </summary>
public sealed class EventContext
{
    /// <summary>Transport-level metadata for the event being handled.</summary>
    public required EventMetadata Metadata { get; init; }

    /// <summary>Scoped service provider for the handler execution.</summary>
    public required IServiceProvider ServiceProvider { get; init; }

    /// <summary>Cancellation token for cooperative shutdown.</summary>
    public CancellationToken CancellationToken { get; init; }
}
