namespace LHA.BackgroundWorker;

/// <summary>
/// Manages the lifecycle of registered <see cref="IBackgroundWorker"/> instances.
/// Coordinates starting and stopping of all workers.
/// </summary>
public interface IBackgroundWorkerManager
{
    /// <summary>
    /// Registers a new worker. If the manager is already running,
    /// the worker is started immediately.
    /// </summary>
    Task AddAsync(IBackgroundWorker worker, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts all registered workers.
    /// </summary>
    Task StartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gracefully stops all registered workers.
    /// </summary>
    Task StopAllAsync(CancellationToken cancellationToken = default);
}
