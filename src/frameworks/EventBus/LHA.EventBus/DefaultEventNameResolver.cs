using System.Collections.Concurrent;
using System.Reflection;

namespace LHA.EventBus;

/// <summary>
/// Default <see cref="IEventNameResolver"/> that uses <see cref="EventNameAttribute"/>
/// and <see cref="EventVersionAttribute"/> with assembly scanning.
/// Falls back to the full type name when no attribute is present.
/// </summary>
public sealed class DefaultEventNameResolver : IEventNameResolver
{
    private readonly ConcurrentDictionary<Type, string> _typeToName = new();
    private readonly ConcurrentDictionary<string, Type?> _nameToType = new();
    private readonly ConcurrentDictionary<Type, int> _typeToVersion = new();

    /// <inheritdoc />
    public string GetName(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        return _typeToName.GetOrAdd(eventType, static type =>
        {
            var attr = type.GetCustomAttribute<EventNameAttribute>();
            return attr?.Name ?? type.FullName ?? type.Name;
        });
    }

    /// <inheritdoc />
    public Type? GetType(string eventName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);

        return _nameToType.GetOrAdd(eventName, static name =>
        {
            // Search all loaded assemblies for the event type
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic) continue;

                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        var attr = type.GetCustomAttribute<EventNameAttribute>();
                        if (attr is not null && attr.Name == name) return type;
                        if (type.FullName == name) return type;
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // Skip assemblies that can't be fully loaded
                }
            }

            return null;
        });
    }

    /// <inheritdoc />
    public int GetVersion(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        return _typeToVersion.GetOrAdd(eventType, static type =>
        {
            var attr = type.GetCustomAttribute<EventVersionAttribute>();
            return attr?.Version ?? 1;
        });
    }
}
