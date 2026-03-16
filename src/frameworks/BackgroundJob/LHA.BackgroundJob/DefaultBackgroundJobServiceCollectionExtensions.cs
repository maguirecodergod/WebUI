using LHA.BackgroundWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.BackgroundJob;

/// <summary>
/// Extension methods for registering the default store-based background job system.
/// </summary>
public static class DefaultBackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Adds the default store-based background job system.
    /// Jobs are serialized → persisted to <see cref="IBackgroundJobStore"/> →
    /// polled by <see cref="BackgroundJobWorker"/> → executed.
    /// By default uses <see cref="InMemoryBackgroundJobStore"/> —
    /// replace with a database-backed store for production.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configure worker options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHADefaultBackgroundJobs(
        this IServiceCollection services,
        Action<BackgroundJobWorkerOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLHABackgroundJobs();
        services.AddLHABackgroundWorkerHosting();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Replace the null manager with the real store-based manager
        services.AddSingleton<IBackgroundJobManager, DefaultBackgroundJobManager>();

        // Store + serializer
        services.TryAddSingleton<IBackgroundJobStore, InMemoryBackgroundJobStore>();
        services.TryAddSingleton<IBackgroundJobSerializer, JsonBackgroundJobSerializer>();

        // Register the worker
        services.AddSingleton<BackgroundJobWorker>();
        services.AddSingleton<IBackgroundWorker>(sp => sp.GetRequiredService<BackgroundJobWorker>());

        return services;
    }
}
