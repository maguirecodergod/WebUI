using System.Reflection;

namespace LHA.BackgroundJob;

/// <summary>
/// Assigns a custom name to a background job arguments type.
/// If not specified, the type's full name is used.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class BackgroundJobNameAttribute : Attribute
{
    /// <summary>The custom job name.</summary>
    public string Name { get; }

    public BackgroundJobNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    /// <summary>
    /// Gets the job name for the given args type.
    /// Returns the <see cref="BackgroundJobNameAttribute.Name"/> if present,
    /// otherwise the type's <see cref="Type.FullName"/>.
    /// </summary>
    public static string GetName<TArgs>() => GetName(typeof(TArgs));

    /// <summary>
    /// Gets the job name for the given args type.
    /// </summary>
    public static string GetName(Type argsType)
    {
        ArgumentNullException.ThrowIfNull(argsType);

        var attribute = argsType.GetCustomAttribute<BackgroundJobNameAttribute>();
        return attribute?.Name ?? argsType.FullName!;
    }
}
