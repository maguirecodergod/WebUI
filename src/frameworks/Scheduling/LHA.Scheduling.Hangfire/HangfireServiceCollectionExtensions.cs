using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Scheduling.Hangfire;

/// <summary>
/// Extension methods for registering the LHA Hangfire Scheduling framework
/// in the dependency injection container.
/// </summary>
public static class HangfireServiceCollectionExtensions
{
    /// <summary>
    /// Adds LHA Scheduling services backed by Hangfire to the service collection.
    /// Registers <see cref="IJobScheduler"/> and <see cref="IRecurringJobManager"/> implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">
    /// Configure Hangfire options. You MUST set <see cref="HangfireSchedulingOptions.ConfigureHangfire"/>
    /// to configure storage (e.g. UsePostgreSqlStorage, UseSqlServerStorage, UseInMemoryStorage).
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHAHangfireScheduling(
        this IServiceCollection services,
        Action<HangfireSchedulingOptions> configure)
    {
        var options = new HangfireSchedulingOptions();
        configure(options);

        services.Configure(configure);

        // Configure Hangfire core
        services.AddHangfire(config =>
        {
            // Let the user configure storage and other Hangfire settings
            options.ConfigureHangfire?.Invoke(config);
        });

        // Configure Hangfire server (processing)
        if (options.EnableServer)
        {
            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.Queues = options.Queues ?? [options.DefaultQueue];

                if (options.WorkerCount.HasValue)
                    serverOptions.WorkerCount = options.WorkerCount.Value;
            });
        }

        // Register LHA abstractions → Hangfire implementations
        services.TryAddSingleton<HangfireJobExecutor>();
        services.TryAddSingleton<IJobScheduler, HangfireJobScheduler>();
        services.TryAddSingleton<IRecurringJobManager, HangfireRecurringJobManager>();
        services.TryAddSingleton<ISchedulingHealthCheck, HangfireHealthCheck>();

        return services;
    }

    /// <summary>
    /// Registers an <see cref="IScheduledJob"/> implementation in DI.
    /// The job will be resolved from scope when executed by the scheduler.
    /// </summary>
    /// <typeparam name="TJob">The job implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddScheduledJob<TJob>(this IServiceCollection services)
        where TJob : class, IScheduledJob
    {
        services.TryAddScoped<TJob>();
        // Also register as the interface type for explicit resolution
        services.TryAddScoped(typeof(IScheduledJob), typeof(TJob));
        return services;
    }
}
