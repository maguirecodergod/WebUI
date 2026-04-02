using LHA.BlazorWasm.Shared.Abstractions.Localization;
using LHA.BlazorWasm.Services.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using LHA.BlazorWasm.Services.Theme;

namespace LHA.BlazorWasm.Components;

/// <summary>
/// Base class for all LHA components to provide common internal injected services.
/// </summary>
public abstract class LhaComponentBase : ComponentBase, IDisposable
{
    [Inject] protected ILocalizationService Localizer { get; set; } = default!;
    [Inject] protected IToastService ToastNotification { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IThemeService ThemeService { get; set; } = default!;
    [Inject] protected ThemeState ThemeState { get; set; } = default!;

    protected override void OnInitialized()
    {
        Localizer.OnLanguageChanged += ReRender;
    }

    private void ReRender() => InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Localizer.OnLanguageChanged -= ReRender;
    }
}
