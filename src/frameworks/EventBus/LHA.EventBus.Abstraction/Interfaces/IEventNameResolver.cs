namespace LHA.EventBus;

/// <summary>
/// Resolves the canonical event name for a given type, and vice-versa.
/// Used by transports for serialization/deserialization routing.
/// </summary>
public interface IEventNameResolver
{
    /// <summary>
    /// Returns the canonical event name for the given type.
    /// Uses <see cref="EventNameAttribute"/> if present, otherwise the full type name.
    /// </summary>
    string GetName(Type eventType);

    /// <summary>
    /// Returns the event <see cref="Type"/> for the given canonical name,
    /// or <c>null</c> if no matching type is registered.
    /// </summary>
    Type? GetType(string eventName);

    /// <summary>
    /// Returns the schema version for the given event type.
    /// Uses <see cref="EventVersionAttribute"/> if present, otherwise returns 1.
    /// </summary>
    int GetVersion(Type eventType);
}
