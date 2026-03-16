namespace LHA.BlazorWasm.UI.Extensions;

/// <summary>
/// Registry for UI extension points.
/// Allows modules to inject components into predefined extension slots.
/// </summary>
public interface IExtensionRegistry
{
    /// <summary>
    /// Register a component for an extension slot.
    /// </summary>
    void RegisterExtension(string slotName, Type componentType);

    /// <summary>
    /// Get all components registered for a specific slot.
    /// </summary>
    IReadOnlyList<Type> GetExtensions(string slotName);

    /// <summary>
    /// Check if a slot has any registered extensions.
    /// </summary>
    bool HasExtensions(string slotName);
}

/// <summary>
/// Default implementation of the extension registry.
/// </summary>
public sealed class ExtensionRegistry : IExtensionRegistry
{
    private readonly Dictionary<string, List<Type>> _extensions = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterExtension(string slotName, Type componentType)
    {
        if (!_extensions.TryGetValue(slotName, out var list))
        {
            list = [];
            _extensions[slotName] = list;
        }
        list.Add(componentType);
    }

    public IReadOnlyList<Type> GetExtensions(string slotName)
    {
        return _extensions.TryGetValue(slotName, out var list)
            ? list.AsReadOnly()
            : Array.Empty<Type>();
    }

    public bool HasExtensions(string slotName)
    {
        return _extensions.TryGetValue(slotName, out var list) && list.Count > 0;
    }
}
