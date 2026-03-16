using Microsoft.Extensions.Logging;

namespace LHA.BackgroundWorker;

/// <summary>
/// A background worker that executes work on a periodic timer.
/// Override <see cref="DoWorkAsync"/> to provide the periodic work logic.
/// </summary>
public abstract class PeriodicBackgroundWorker : BackgroundWorkerBase
{
    /// <summary>
    /// The interval between executions. Can be changed at runtime.
    /// </summary>
    public TimeSpan Period { get; set; }

    /// <summary>
    /// Whether the first execution should occur immediately on start,
    /// or wait for the first period to elapse. Default: <see langword="false"/>.
    /// </summary>
    public bool ExecuteOnStart { get; set; }

    protected PeriodicBackgroundWorker(TimeSpan period, ILogger? logger = null)
        : base(logger)
    {
        Period = period;
    }

    /// <inheritdoc />
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!ExecuteOnStart)
        {
            await DelayAsync(stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in periodic worker {WorkerType}. Will retry after {Period}.",
                    GetType().Name, Period);
                await OnErrorAsync(ex, stoppingToken);
            }

            await DelayAsync(stoppingToken);
        }
    }

    /// <summary>
    /// Override to implement the periodic work.
    /// </summary>
    protected abstract Task DoWorkAsync(CancellationToken stoppingToken);

    /// <summary>
    /// Called when <see cref="DoWorkAsync"/> throws. Override to implement
    /// custom error handling (e.g., exponential backoff).
    /// Default implementation does nothing.
    /// </summary>
    protected virtual Task OnErrorAsync(Exception exception, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private async Task DelayAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(Period, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown — do not rethrow
        }
    }
}
