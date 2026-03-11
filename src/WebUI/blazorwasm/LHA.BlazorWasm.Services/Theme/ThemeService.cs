using LHA.BlazorWasm.Services.Storage;

namespace LHA.BlazorWasm.Services.Theme;

public class ThemeService(ThemeState themeState, ILocalStorageService localStorage) : IThemeService
{
    private const string ThemeStorageKey = "app:theme";

    public ThemeMode CurrentTheme => themeState.CurrentTheme;

    public string ThemeClass => CurrentTheme switch
    {
        ThemeMode.Light => "theme-light",
        ThemeMode.Dark => "theme-dark",
        ThemeMode.System => "theme-light", // Assuming default fallback, could be enhanced with media queries
        _ => "theme-light"
    };

    public async Task InitializeAsync()
    {
        var savedThemeStr = await localStorage.GetAsync<string>(ThemeStorageKey);
        
        if (Enum.TryParse<ThemeMode>(savedThemeStr, out var savedTheme))
        {
            themeState.NotifyThemeChanged(savedTheme);
        }
        else
        {
            themeState.NotifyThemeChanged(ThemeMode.System);
        }
    }

    public async Task SetThemeAsync(ThemeMode theme)
    {
        if (themeState.CurrentTheme != theme)
        {
            await localStorage.SetAsync(ThemeStorageKey, theme.ToString());
            themeState.NotifyThemeChanged(theme);
        }
    }
}
