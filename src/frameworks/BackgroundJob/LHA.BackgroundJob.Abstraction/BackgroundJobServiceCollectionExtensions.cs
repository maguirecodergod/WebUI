using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.BackgroundJob;

/// <summary>
/// Extension methods for registering background job abstractions.
/// </summary>
public static class BackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Registers background job core services (options, executer, null manager).
    /// Call this before registering a specific provider.
    /// </summary>
    public static IServiceCollection AddLHABackgroundJobs(
        this IServiceCollection services,
        Action<BackgroundJobOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IBackgroundJobSerializer, JsonBackgroundJobSerializer>();
        services.TryAddSingleton<IBackgroundJobExecuter, BackgroundJobExecuter>();
        services.TryAddSingleton<IBackgroundJobManager, NullBackgroundJobManager>();

        return services;
    }

    /// <summary>
    /// Registers a background job type and its args type.
    /// The job is registered as scoped in DI and added to <see cref="BackgroundJobOptions"/>.
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    public static IServiceCollection AddBackgroundJob<TJob, TArgs>(this IServiceCollection services)
        where TJob : class, IBackgroundJob<TArgs>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<TJob>();

        services.Configure<BackgroundJobOptions>(options =>
        {
            options.AddJob<TJob, TArgs>();
        });

        return services;
    }
}
