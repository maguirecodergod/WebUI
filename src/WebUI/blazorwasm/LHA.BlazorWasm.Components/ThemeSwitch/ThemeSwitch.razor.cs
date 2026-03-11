using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Services.Theme;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.ThemeSwitch;

public class ThemeSwitchBase : ComponentBase, IDisposable
{
    [Inject] protected IThemeService ThemeService { get; set; } = default!;
    [Inject] protected ThemeState ThemeState { get; set; } = default!;
    [Inject] protected ILocalizationService LocalizationService { get; set; } = default!;

    [Parameter] public ThemeSwitchVariant Variant { get; set; } = ThemeSwitchVariant.Toggle;
    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Style { get; set; } = string.Empty;
    [Parameter] public bool ShowLabel { get; set; }
    [Parameter] public string Size { get; set; } = "md"; // "sm", "md", "lg"

    protected ThemeMode CurrentTheme => ThemeState.CurrentTheme;

    protected string VariantClass => Variant == ThemeSwitchVariant.Toggle ? "ts-toggle" : "ts-icon";
    protected string SizeClass => $"ts-{Size}";

    protected override void OnInitialized()
    {
        ThemeState.OnThemeChanged += OnStateChanged;
        LocalizationService.OnLanguageChanged += RefreshUI;
    }

    private void OnStateChanged(ThemeMode newTheme)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        InvokeAsync(StateHasChanged);
    }

    protected async Task ToggleTheme()
    {
        var nextTheme = CurrentTheme == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light;
        await ThemeService.SetThemeAsync(nextTheme);
    }

    public void Dispose()
    {
        ThemeState.OnThemeChanged -= OnStateChanged;
        LocalizationService.OnLanguageChanged -= RefreshUI;
    }
}
