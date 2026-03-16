namespace LHA.BlazorWasm.Services.Theme;

public class ThemeState
{
    public CThemeMode CurrentTheme { get; set; } = CThemeMode.System;

    public event Action<CThemeMode>? OnThemeChanged;

    public void NotifyThemeChanged(CThemeMode newTheme)
    {
        CurrentTheme = newTheme;
        OnThemeChanged?.Invoke(newTheme);
    }
}
