using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using LHA.BlazorWasm.Shared.Models.Localization;

namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Dependency Injection extension methods for the Localization service.
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Adds the application's localization service with default or customized module/JSON resource options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional configuration action to append Module resource paths natively.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAppLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            // Register defaults if none dynamically applied
            services.Configure<LocalizationOptions>(options => { });
        }

        services.AddSingleton<LocalizationState>();
        services.AddScoped<ILocalizationService, LocalizationService>();

        return services;
    }
}
