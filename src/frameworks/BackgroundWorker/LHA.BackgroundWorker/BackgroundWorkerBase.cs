using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.BackgroundWorker;

/// <summary>
/// Base class for background workers with built-in logging and cancellation support.
/// Subclasses override <see cref="ExecuteAsync"/> to provide the work loop.
/// </summary>
public abstract class BackgroundWorkerBase : IBackgroundWorker, IDisposable
{
    private CancellationTokenSource? _stoppingCts;
    private Task? _executingTask;
    private bool _disposed;

    protected ILogger Logger { get; }

    /// <summary>
    /// A token that is triggered when <see cref="StopAsync"/> is called.
    /// </summary>
    protected CancellationToken StoppingToken => _stoppingCts?.Token ?? CancellationToken.None;

    protected BackgroundWorkerBase(ILogger? logger = null)
    {
        Logger = logger ?? NullLogger.Instance;
    }

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        Logger.LogDebug("Starting background worker {WorkerType}.", GetType().Name);
        _executingTask = ExecuteAsync(_stoppingCts.Token);

        // If the task completed synchronously, return it directly
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_executingTask is null) return;

        Logger.LogDebug("Stopping background worker {WorkerType}.", GetType().Name);

        try
        {
            _stoppingCts?.Cancel();
        }
        finally
        {
            // Wait for the worker to complete or the shutdown token to fire
            var tcs = new TaskCompletionSource<object?>();
            await using var reg = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            await Task.WhenAny(_executingTask, tcs.Task);
        }
    }

    /// <summary>
    /// Override to implement the long-running background work.
    /// The method should respect the <paramref name="stoppingToken"/> for graceful shutdown.
    /// </summary>
    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stoppingCts?.Cancel();
        _stoppingCts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
