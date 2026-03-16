using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LHA.BackgroundWorker.Quartz;

/// <summary>
/// Extension methods for registering Quartz-based background workers.
/// </summary>
public static class QuartzBackgroundWorkerServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="QuartzPeriodicBackgroundWorker"/> implementation.
    /// The worker is automatically started/stopped with the host lifecycle
    /// and uses Quartz.NET for persistent, cluster-safe cron scheduling.
    /// </summary>
    /// <typeparam name="TWorker">The concrete worker type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddLHAQuartzScheduling(options => { ... });
    /// services.AddLHAQuartzBackgroundWorker&lt;CleanupWorker&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddLHAQuartzBackgroundWorker<TWorker>(
        this IServiceCollection services)
        where TWorker : QuartzPeriodicBackgroundWorker
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<TWorker>();
        services.AddHostedService<QuartzWorkerHostedService<TWorker>>();
        return services;
    }
}

/// <summary>
/// Hosted service wrapper that manages the lifecycle of a single
/// <see cref="QuartzPeriodicBackgroundWorker"/> within the .NET Generic Host.
/// </summary>
internal sealed class QuartzWorkerHostedService<TWorker> : IHostedService
    where TWorker : QuartzPeriodicBackgroundWorker
{
    private readonly TWorker _worker;

    public QuartzWorkerHostedService(TWorker worker) => _worker = worker;

    public Task StartAsync(CancellationToken cancellationToken) =>
        _worker.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        _worker.StopAsync(cancellationToken);
}
