namespace LHA.BlazorWasm.Components.LanguageSelector;

/// <summary>
/// Defines a language option for the LanguageSelector.
/// </summary>
public class LanguageOption
{
    /// <summary>
    /// The culture code (e.g., "en", "vi").
    /// </summary>
    public string Culture { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the language (e.g., "English", "Tiếng Việt").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The string or emoji representing the flag (e.g., "🇺🇸").
    /// </summary>
    public string Flag { get; set; } = string.Empty;
}
