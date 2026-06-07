namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Defines the visual mode for the LanguageSelector component.
/// </summary>
public enum CLanguageSelectorMode
{
    /// <summary>
    /// 0 - Dropdown: Renders a dropdown menu to select the language.
    /// </summary>
    Dropdown,

    /// <summary>
    /// 1 - Inline: Renders a series of inline toggle buttons.
    /// </summary>
    Inline
}
