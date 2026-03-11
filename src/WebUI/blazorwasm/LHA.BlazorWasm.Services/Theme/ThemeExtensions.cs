using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.Theme;

public static class ThemeExtensions
{
    public static IServiceCollection AddThemeService(this IServiceCollection services)
    {
        services.AddScoped<ThemeState>();
        services.AddScoped<IThemeService, ThemeService>();
        
        return services;
    }
}
