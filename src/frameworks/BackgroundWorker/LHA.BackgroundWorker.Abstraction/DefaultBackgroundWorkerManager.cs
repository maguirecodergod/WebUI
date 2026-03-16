using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.BackgroundWorker;

/// <summary>
/// Thread-safe manager that coordinates the lifecycle of registered <see cref="IBackgroundWorker"/> instances.
/// </summary>
internal sealed class DefaultBackgroundWorkerManager : IBackgroundWorkerManager, IDisposable
{
    private readonly List<IBackgroundWorker> _workers = [];
    private readonly Lock _lock = new();
    private readonly ILogger<DefaultBackgroundWorkerManager> _logger;
    private bool _isRunning;
    private bool _isDisposed;

    public DefaultBackgroundWorkerManager(ILogger<DefaultBackgroundWorkerManager>? logger = null)
    {
        _logger = logger ?? NullLogger<DefaultBackgroundWorkerManager>.Instance;
    }

    /// <inheritdoc />
    public async Task AddAsync(IBackgroundWorker worker, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(worker);

        lock (_lock)
        {
            _workers.Add(worker);
        }

        if (_isRunning)
        {
            _logger.LogDebug("Starting late-registered worker {WorkerType}.", worker.GetType().Name);
            await worker.StartAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task StartAllAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = true;

        List<IBackgroundWorker> snapshot;
        lock (_lock)
        {
            snapshot = [.. _workers];
        }

        foreach (var worker in snapshot)
        {
            _logger.LogDebug("Starting worker {WorkerType}.", worker.GetType().Name);
            await worker.StartAsync(cancellationToken);
        }

        _logger.LogInformation("Started {Count} background worker(s).", snapshot.Count);
    }

    /// <inheritdoc />
    public async Task StopAllAsync(CancellationToken cancellationToken = default)
    {
        _isRunning = false;

        List<IBackgroundWorker> snapshot;
        lock (_lock)
        {
            snapshot = [.. _workers];
        }

        // Stop in reverse order for graceful teardown
        for (var i = snapshot.Count - 1; i >= 0; i--)
        {
            try
            {
                _logger.LogDebug("Stopping worker {WorkerType}.", snapshot[i].GetType().Name);
                await snapshot[i].StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping worker {WorkerType}.", snapshot[i].GetType().Name);
            }
        }

        _logger.LogInformation("Stopped {Count} background worker(s).", snapshot.Count);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        lock (_lock)
        {
            foreach (var worker in _workers)
            {
                (worker as IDisposable)?.Dispose();
            }
            _workers.Clear();
        }
    }
}
