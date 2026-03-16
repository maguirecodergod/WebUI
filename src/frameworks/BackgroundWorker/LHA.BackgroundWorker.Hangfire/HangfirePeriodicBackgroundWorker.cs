using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.BackgroundWorker.Hangfire;

/// <summary>
/// Base class for background workers scheduled by Hangfire.
/// Provides persistent, distributed periodic execution with cron-based scheduling —
/// replacing the simple timer-based <c>PeriodicBackgroundWorker</c>.
/// </summary>
/// <remarks>
/// On <see cref="StartAsync"/>, the worker registers a Hangfire recurring job.
/// When Hangfire fires the job, <see cref="HangfireWorkerJobExecutor"/> resolves
/// this worker from DI and calls <see cref="DoWorkAsync"/>.
/// On <see cref="StopAsync"/>, the recurring job is removed.
/// </remarks>
/// <example>
/// <code>
/// public sealed class CleanupWorker : HangfirePeriodicBackgroundWorker
/// {
///     protected override string CronExpression => "*/5 * * * *";
///
///     public CleanupWorker(
///         Hangfire.IRecurringJobManager rm,
///         ILogger&lt;CleanupWorker&gt; logger)
///         : base(rm, logger) { }
///
///     public override async Task DoWorkAsync(CancellationToken cancellationToken)
///     {
///         // periodic cleanup logic
///     }
/// }
///
/// // Registration:
/// services.AddLHAHangfireBackgroundWorker&lt;CleanupWorker&gt;();
/// </code>
/// </example>
public abstract class HangfirePeriodicBackgroundWorker : IBackgroundWorker, IDisposable
{
    private readonly global::Hangfire.IRecurringJobManager _recurringJobManager;

    /// <summary>Logger for the worker.</summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Cron expression defining the execution schedule.
    /// Use <c>LHA.Scheduling.CronPresets</c> for common patterns.
    /// </summary>
    protected abstract string CronExpression { get; }

    /// <summary>
    /// Unique recurring job ID. Default: the worker type's full name.
    /// </summary>
    protected virtual string RecurringJobId => GetType().FullName!;

    /// <summary>
    /// Hangfire queue name. Default: <c>"default"</c>.
    /// </summary>
    protected virtual string Queue => "default";

    /// <summary>
    /// Time zone for cron expression evaluation. Default: <see cref="TimeZoneInfo.Utc"/>.
    /// </summary>
    protected virtual TimeZoneInfo TimeZone => TimeZoneInfo.Utc;

    /// <summary>
    /// Creates a new Hangfire-based periodic background worker.
    /// </summary>
    /// <param name="recurringJobManager">Hangfire recurring job manager (injected by DI).</param>
    /// <param name="logger">Optional logger; falls back to <see cref="NullLogger"/>.</param>
    protected HangfirePeriodicBackgroundWorker(
        global::Hangfire.IRecurringJobManager recurringJobManager,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(recurringJobManager);
        _recurringJobManager = recurringJobManager;
        Logger = logger ?? NullLogger.Instance;
    }

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        var workerTypeName = GetType().AssemblyQualifiedName!;

        _recurringJobManager.AddOrUpdate<HangfireWorkerJobExecutor>(
            RecurringJobId,
            Queue,
            executor => executor.ExecuteAsync(workerTypeName, CancellationToken.None),
            CronExpression,
            new global::Hangfire.RecurringJobOptions { TimeZone = TimeZone });

        Logger.LogInformation(
            "Registered Hangfire recurring job '{RecurringJobId}' for worker {WorkerType} with cron '{CronExpression}'.",
            RecurringJobId, GetType().Name, CronExpression);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _recurringJobManager.RemoveIfExists(RecurringJobId);
            Logger.LogDebug("Removed Hangfire recurring job '{RecurringJobId}'.", RecurringJobId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.LogWarning(ex,
                "Failed to remove Hangfire recurring job '{RecurringJobId}'.",
                RecurringJobId);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Override to implement the periodic work logic.
    /// Called each time Hangfire fires the recurring job.
    /// </summary>
    /// <param name="cancellationToken">Token signaled when the job should abort.</param>
    public abstract Task DoWorkAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
