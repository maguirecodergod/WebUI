using System.Reflection;

namespace LHA.Localization.Abstraction;

/// <summary>
/// Describes a registered localization resource, including its type,
/// assembly, base namespace path, and inheritance chain.
/// </summary>
public sealed class LocalizationResourceDescriptor
{
    /// <summary>
    /// The resource marker type (e.g. typeof(MultiTenancyResource)).
    /// </summary>
    public required Type ResourceType { get; init; }

    /// <summary>
    /// The assembly that contains the embedded resource files.
    /// </summary>
    public Assembly Assembly => ResourceType.Assembly;

    /// <summary>
    /// The base namespace path for embedded resources.
    /// Computed from the resource type's namespace + "Localization" folder convention.
    /// </summary>
    public required string ResourceBasePath { get; init; }

    /// <summary>
    /// The display name of this resource. Defaults to the type name.
    /// </summary>
    public required string ResourceName { get; init; }

    /// <summary>
    /// The default/fallback culture for this resource.
    /// </summary>
    public required string DefaultCulture { get; init; }

    /// <summary>
    /// Base resource types that this resource inherits from.
    /// Strings from base resources are used as fallback when not found in this resource.
    /// </summary>
    public IReadOnlyList<Type> BaseResourceTypes { get; init; } = [];
}
