using LHA.BlazorWasm.Services.Storage;

namespace LHA.BlazorWasm.Services.Theme;

internal sealed class ThemeService(ThemeState themeState, ILocalStorageService localStorage) : IThemeService
{
    private const string ThemeStorageKey = "app:theme";

    public CThemeMode CurrentTheme => themeState.CurrentTheme;

    public string ThemeClass => CurrentTheme switch
    {
        CThemeMode.Light => "theme-light",
        CThemeMode.Dark => "theme-dark",
        CThemeMode.System => "theme-light", // Assuming default fallback, could be enhanced with media queries
        _ => "theme-light"
    };

    public async Task InitializeAsync()
    {
        var savedThemeStr = await localStorage.GetAsync<string>(ThemeStorageKey);

        if (Enum.TryParse<CThemeMode>(savedThemeStr, out var savedTheme))
        {
            themeState.NotifyThemeChanged(savedTheme);
        }
        else
        {
            themeState.NotifyThemeChanged(CThemeMode.System);
        }
    }

    public async Task SetThemeAsync(CThemeMode theme)
    {
        if (themeState.CurrentTheme != theme)
        {
            await localStorage.SetAsync(ThemeStorageKey, theme.ToString());
            themeState.NotifyThemeChanged(theme);
        }
    }
}
