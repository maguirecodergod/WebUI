using System.ComponentModel;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace LHA.BackgroundWorker.Hangfire;

/// <summary>
/// Hangfire-invoked service that resolves a <see cref="HangfirePeriodicBackgroundWorker"/>
/// by type name and delegates execution. Hangfire serializes the method call to this executor;
/// when the recurring job fires, Hangfire resolves this class from DI and invokes
/// <see cref="ExecuteAsync"/>.
/// </summary>
public sealed class HangfireWorkerJobExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HangfireWorkerJobExecutor> _logger;

    public HangfireWorkerJobExecutor(
        IServiceProvider serviceProvider,
        ILogger<HangfireWorkerJobExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Called by Hangfire when a recurring worker job fires.
    /// Resolves the worker from DI and calls <see cref="HangfirePeriodicBackgroundWorker.DoWorkAsync"/>.
    /// </summary>
    /// <param name="workerTypeName">Assembly-qualified type name of the worker.</param>
    /// <param name="cancellationToken">Hangfire cancellation token (signals job abort).</param>
    [AutomaticRetry(Attempts = 0)]
    [DisplayName("BackgroundWorker: {0}")]
    public async Task ExecuteAsync(string workerTypeName, CancellationToken cancellationToken)
    {
        var workerType = Type.GetType(workerTypeName)
            ?? throw new InvalidOperationException(
                $"Cannot resolve worker type: {workerTypeName}");

        var worker = _serviceProvider.GetService(workerType) as HangfirePeriodicBackgroundWorker
            ?? throw new InvalidOperationException(
                $"Service of type {workerType.Name} is not registered " +
                $"or is not a {nameof(HangfirePeriodicBackgroundWorker)}.");

        _logger.LogDebug("Hangfire firing background worker {WorkerType}.", workerType.Name);

        await worker.DoWorkAsync(cancellationToken);
    }
}
