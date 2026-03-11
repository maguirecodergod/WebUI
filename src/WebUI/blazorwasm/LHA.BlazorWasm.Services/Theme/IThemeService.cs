namespace LHA.BlazorWasm.Services.Theme;

public interface IThemeService
{
    ThemeMode CurrentTheme { get; }
    string ThemeClass { get; }
    
    Task InitializeAsync();
    Task SetThemeAsync(ThemeMode theme);
}
