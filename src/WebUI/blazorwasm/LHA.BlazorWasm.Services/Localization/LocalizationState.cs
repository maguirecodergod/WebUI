namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Container holding the active Localization state (Culture and active Translations).
/// Primarily used as a reactive record.
/// </summary>
public class LocalizationState
{
    /// <summary>
    /// The currently active culture string (e.g., "en", "vi").
    /// </summary>
    public string CurrentCulture { get; set; } = string.Empty;

    /// <summary>
    /// The flat dictionary holding computed dot-notation keys (e.g., "User.Title") mapping to translated values.
    /// </summary>
    public Dictionary<string, string> Translations { get; set; } = new();
}
