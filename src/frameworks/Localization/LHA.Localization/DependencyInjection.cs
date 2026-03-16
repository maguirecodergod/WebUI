using LHA.Localization.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace LHA.Localization;

/// <summary>
/// Extension methods for registering the LHA Localization framework
/// in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds LHA Localization services to the service collection.
    /// Registers the resource reader, resource manager, string localizer factory,
    /// and generic IStringLocalizer&lt;T&gt;.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to configure localization resources.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHALocalization(
        this IServiceCollection services,
        Action<LocalizationResourceOptions>? configure = null)
    {
        // Configure options
        if (configure is not null)
            services.Configure(configure);

        // Core services
        services.TryAddSingleton<ILocalizationResourceReader, EmbeddedJsonLocalizationResourceReader>();
        services.TryAddSingleton<LocalizationResourceManager>();

        // Factory (replaces default MS implementation)
        services.TryAddSingleton<ILHAStringLocalizerFactory, LHAStringLocalizerFactory>();
        services.TryAddSingleton<IStringLocalizerFactory>(sp =>
            sp.GetRequiredService<ILHAStringLocalizerFactory>());

        // Generic IStringLocalizer<T> registration
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(LHAStringLocalizer<>));

        return services;
    }
}
