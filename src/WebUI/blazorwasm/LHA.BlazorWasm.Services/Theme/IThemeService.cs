namespace LHA.BlazorWasm.Services.Theme;

public interface IThemeService
{
    CThemeMode CurrentTheme { get; }
    string ThemeClass { get; }
    
    Task InitializeAsync();
    Task SetThemeAsync(CThemeMode theme);
}
