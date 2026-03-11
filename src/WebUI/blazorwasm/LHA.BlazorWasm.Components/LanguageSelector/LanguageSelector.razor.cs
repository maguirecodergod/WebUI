using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LHA.BlazorWasm.Services.Localization;
using Microsoft.JSInterop;

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
public partial class LanguageSelector : ComponentBase, IDisposable, IAsyncDisposable
{
    [Inject] private ILocalizationService LocalizationService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

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

    /// <summary>
    /// Private state to control internal Dropdown visibility natively without reliance on JS.
    /// </summary>
    private bool _isDropdownOpen;
    private bool _shouldAlignDropdown;
    private ElementReference _containerRef;
    private IJSObjectReference? _jsModule;

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
            l.Culture.Equals(LocalizationService.State.CurrentCulture, StringComparison.OrdinalIgnoreCase))
        ?? SupportedOptions.FirstOrDefault();

    protected override void OnInitialized()
    {
        // Subscribe to localization service to trigger re-renders dynamically when language changes
        LocalizationService.OnLanguageChanged += RefreshUI;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Mode == LanguageSelectorMode.Dropdown)
        {
            try
            {
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/LHA.BlazorWasm.Components/LanguageSelector/LanguageSelector.razor.js");
            }
            catch { }
        }

        if (_shouldAlignDropdown && _jsModule != null)
        {
            _shouldAlignDropdown = false;
            try
            {
                await _jsModule.InvokeVoidAsync("autoAlignDropdown", _containerRef);
            }
            catch { }
        }
    }

    private void RefreshUI()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task SelectLanguageAsync(string culture)
    {
        if (LocalizationService.State.CurrentCulture != culture)
        {
            await LocalizationService.SetLanguageAsync(culture);
            _isDropdownOpen = false;
        }
    }

    private void ToggleDropdown()
    {
        _isDropdownOpen = !_isDropdownOpen;
        if (_isDropdownOpen)
        {
            _shouldAlignDropdown = true;
        }
    }

    private void CloseDropdown()
    {
        _isDropdownOpen = false;
    }

    public void Dispose()
    {
        LocalizationService.OnLanguageChanged -= RefreshUI;
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.DisposeAsync();
            }
            catch { }
        }
    }
}
