using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Components.Select;

namespace LHA.BlazorWasm.Components.LanguageSelector;

/// <summary>
/// A reusable enterprise-grade Language Selector component.
/// Displays available languages and switches culture natively.
/// 
/// Example usage:
/// 
/// Dropdown:
/// <LanguageSelector Mode="LanguageSelectorMode.Dropdown" />
/// 
/// Inline:
/// <LanguageSelector Mode="LanguageSelectorMode.Inline" />
/// </summary>
public partial class LanguageSelector : LhaComponentBase, IDisposable
{
    /// <summary>
    /// Gets or sets the visual variant mode of the selector (Dropdown or Inline).
    /// </summary>
    [Parameter] public LanguageSelectorMode Mode { get; set; } = LanguageSelectorMode.Dropdown;

    /// <summary>
    /// Defines if the flag icon should be shown.
    /// </summary>
    [Parameter] public bool ShowFlags { get; set; } = true;

    /// <summary>
    /// Defines if the language name text should be shown.
    /// </summary>
    [Parameter] public bool ShowNames { get; set; } = true;

    /// <summary>
    /// Additional custom CSS classes.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Additional inline style properties.
    /// </summary>
    [Parameter] public string? Style { get; set; }

    private ICollection<SelectOption<string>> SelectOptions =>
        SupportedOptions.Select(o => new SelectOption<string>
        {
            Value = o.Culture,
            Label = o.Name
        }).ToList();

    private string? CurrentCulture
    {
        get => Localizer.State.CurrentCulture;
        set { if (value != null) _ = SelectLanguageAsync(value); }
    }

    /// <summary>
    /// The comprehensive list of worldly language enums this selector displays.
    /// By default, it will fall back to English and Vietnamese if not explicitly provided.
    /// </summary>
    [Parameter]
    public IEnumerable<LanguageCode> SupportedLanguages { get; set; } = new[]
    {
        LanguageCode.EN,
        LanguageCode.VI
    };

    private IReadOnlyList<LanguageOption> SupportedOptions =>
        LanguageProvider.GetOptions(SupportedLanguages);

    private LanguageOption? CurrentLanguage =>
        SupportedOptions.FirstOrDefault(l =>
            l.Culture.Equals(Localizer.State.CurrentCulture, StringComparison.OrdinalIgnoreCase))
        ?? SupportedOptions.FirstOrDefault();

    protected override void OnInitialized()
    {
        // Subscribe to localization service to trigger re-renders dynamically when language changes
        Localizer.OnLanguageChanged += RefreshUI;
    }

    private void RefreshUI()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task SelectLanguageAsync(string culture)
    {
        if (Localizer.State.CurrentCulture != culture)
        {
            await Localizer.SetLanguageAsync(culture);
        }
    }

    public void Dispose()
    {
        Localizer.OnLanguageChanged -= RefreshUI;
    }
}
