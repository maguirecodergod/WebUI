using Microsoft.Extensions.DependencyInjection;

namespace LHA.BackgroundWorker;

/// <summary>
/// Extension methods for registering the background worker hosted service.
/// </summary>
public static class BackgroundWorkerHostingExtensions
{
    /// <summary>
    /// Registers background worker infrastructure with the Generic Host.
    /// Workers are started/stopped automatically with the host lifecycle.
    /// </summary>
    public static IServiceCollection AddLHABackgroundWorkerHosting(
        this IServiceCollection services,
        Action<BackgroundWorkerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLHABackgroundWorker(configure);
        services.AddHostedService<BackgroundWorkerHostedService>();

        return services;
    }
}
