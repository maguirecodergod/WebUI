namespace LHA.EventBus;

/// <summary>
/// Transport envelope wrapping a deserialized event payload with its metadata.
/// Used by event handlers to access both the business event and transport-level information.
/// </summary>
/// <typeparam name="TEvent">The event payload type.</typeparam>
public sealed record EventEnvelope<TEvent>(TEvent Payload, EventMetadata Metadata)
    where TEvent : class;

/// <summary>
/// Non-generic envelope for scenarios where the event type is not known at compile time.
/// </summary>
public sealed record EventEnvelope(object Payload, EventMetadata Metadata);
