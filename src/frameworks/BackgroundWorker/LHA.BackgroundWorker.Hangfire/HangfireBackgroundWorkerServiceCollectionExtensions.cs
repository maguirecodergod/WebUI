using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LHA.BackgroundWorker.Hangfire;

/// <summary>
/// Extension methods for registering Hangfire-based background workers.
/// </summary>
public static class HangfireBackgroundWorkerServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="HangfirePeriodicBackgroundWorker"/> implementation.
    /// The worker is automatically started/stopped with the host lifecycle
    /// and uses Hangfire for persistent, distributed cron scheduling.
    /// </summary>
    /// <typeparam name="TWorker">The concrete worker type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <remarks>
    /// Requires Hangfire to be configured via <c>AddLHAHangfireScheduling</c>
    /// or equivalent Hangfire DI registration.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddLHAHangfireScheduling(options => { ... });
    /// services.AddLHAHangfireBackgroundWorker&lt;CleanupWorker&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddLHAHangfireBackgroundWorker<TWorker>(
        this IServiceCollection services)
        where TWorker : HangfirePeriodicBackgroundWorker
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<HangfireWorkerJobExecutor>();
        services.TryAddSingleton<TWorker>();
        services.AddHostedService<HangfireWorkerHostedService<TWorker>>();
        return services;
    }
}

/// <summary>
/// Hosted service wrapper that manages the lifecycle of a single
/// <see cref="HangfirePeriodicBackgroundWorker"/> within the .NET Generic Host.
/// </summary>
internal sealed class HangfireWorkerHostedService<TWorker> : IHostedService
    where TWorker : HangfirePeriodicBackgroundWorker
{
    private readonly TWorker _worker;

    public HangfireWorkerHostedService(TWorker worker) => _worker = worker;

    public Task StartAsync(CancellationToken cancellationToken) =>
        _worker.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        _worker.StopAsync(cancellationToken);
}
