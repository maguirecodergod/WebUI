namespace LHA.Localization.Abstraction;

/// <summary>
/// Extension point that contributes additional localization key-value pairs
/// for a specific culture. Implementations can load strings from custom sources
/// such as databases, remote services, or additional file formats.
/// </summary>
public interface ILocalizationResourceContributor
{
    /// <summary>
    /// Returns localization entries for the specified culture.
    /// </summary>
    /// <param name="cultureName">The culture name (e.g. "en", "vi", "ja").</param>
    /// <returns>
    /// A dictionary of localization key-value pairs, or null if this
    /// contributor has no entries for the given culture.
    /// </returns>
    Task<IReadOnlyDictionary<string, string>?> GetStringsAsync(string cultureName);

    /// <summary>
    /// Returns the resource type this contributor is associated with.
    /// </summary>
    Type ResourceType { get; }
}
