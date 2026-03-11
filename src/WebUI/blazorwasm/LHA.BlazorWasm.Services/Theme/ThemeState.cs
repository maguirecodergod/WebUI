namespace LHA.BlazorWasm.Services.Theme;

public class ThemeState
{
    public ThemeMode CurrentTheme { get; set; } = ThemeMode.System;

    public event Action<ThemeMode>? OnThemeChanged;

    public void NotifyThemeChanged(ThemeMode newTheme)
    {
        CurrentTheme = newTheme;
        OnThemeChanged?.Invoke(newTheme);
    }
}
