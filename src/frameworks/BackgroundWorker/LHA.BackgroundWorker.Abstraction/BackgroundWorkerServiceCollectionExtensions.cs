using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.BackgroundWorker;

/// <summary>
/// Extension methods for registering background worker abstractions.
/// </summary>
public static class BackgroundWorkerServiceCollectionExtensions
{
    /// <summary>
    /// Registers the background worker infrastructure.
    /// </summary>
    public static IServiceCollection AddLHABackgroundWorker(
        this IServiceCollection services,
        Action<BackgroundWorkerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IBackgroundWorkerManager, DefaultBackgroundWorkerManager>();

        return services;
    }
}
