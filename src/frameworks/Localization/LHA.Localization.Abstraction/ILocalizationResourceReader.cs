using System.Reflection;

namespace LHA.Localization.Abstraction;

/// <summary>
/// Reads localization key-value dictionaries from a specific source format.
/// The default implementation reads embedded JSON resources from assemblies.
/// </summary>
public interface ILocalizationResourceReader
{
    /// <summary>
    /// Reads localization strings for a given culture from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resources.</param>
    /// <param name="resourceBasePath">
    /// The base path/namespace for the resource (e.g. "LHA.MultiTenancy.Abstraction.Localization").
    /// </param>
    /// <param name="cultureName">The culture name (e.g. "en", "vi").</param>
    /// <returns>
    /// A dictionary of key-value pairs read from the resource, or null if the
    /// resource for the specified culture was not found.
    /// </returns>
    IReadOnlyDictionary<string, string>? ReadStrings(
        Assembly assembly,
        string resourceBasePath,
        string cultureName);
}
