using LHA.BlazorWasm.Services.Theme;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.App.Components.Base;

public abstract class LHALayoutComponentBase : LayoutComponentBase, IDisposable
{
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected IThemeService ThemeService { get; set; } = default!;
    [Inject] protected ThemeState ThemeState { get; set; } = default!;
    [Inject] protected ILocalizationService Localizer { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ThemeState.OnThemeChanged += HandleThemeChanged;
        Localizer.OnLanguageChanged += HandleLanguageChanged;
    }

    protected virtual void HandleThemeChanged(CThemeMode mode)
    {
        InvokeAsync(StateHasChanged);
    }

    protected virtual void HandleLanguageChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public virtual void Dispose()
    {
        ThemeState.OnThemeChanged -= HandleThemeChanged;
        Localizer.OnLanguageChanged -= HandleLanguageChanged;
    }
}
