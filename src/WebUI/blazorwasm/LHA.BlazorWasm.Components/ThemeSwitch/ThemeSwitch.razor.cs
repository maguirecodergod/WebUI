using LHA.BlazorWasm.Services.Theme;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.ThemeSwitch;

public class ThemeSwitchBase : LhaComponentBase, IDisposable
{
    [Parameter] public CThemeSwitchVariant Variant { get; set; } = CThemeSwitchVariant.Toggle;
    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Style { get; set; } = string.Empty;
    [Parameter] public bool ShowLabel { get; set; }
    [Parameter] public string Size { get; set; } = "md"; // "sm", "md", "lg"

    protected CThemeMode CurrentTheme => ThemeState.CurrentTheme;

    protected string VariantClass => Variant == CThemeSwitchVariant.Toggle ? "ts-toggle" : "ts-icon";
    protected string SizeClass => $"ts-{Size}";

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ThemeState.OnThemeChanged += OnStateChanged;
    }

    private void OnStateChanged(CThemeMode newTheme)
    {
        InvokeAsync(StateHasChanged);
    }

    protected async Task ToggleTheme()
    {
        var nextTheme = CurrentTheme == CThemeMode.Light ? CThemeMode.Dark : CThemeMode.Light;
        await ThemeService.SetThemeAsync(nextTheme);
    }

    public override void Dispose()
    {
        base.Dispose();
        ThemeState.OnThemeChanged -= OnStateChanged;
    }
}
