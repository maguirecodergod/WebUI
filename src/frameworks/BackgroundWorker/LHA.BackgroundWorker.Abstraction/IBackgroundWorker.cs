namespace LHA.BackgroundWorker;

/// <summary>
/// Defines a background worker that runs independently of request processing.
/// Implementations should be long-lived and resilient to transient failures.
/// </summary>
public interface IBackgroundWorker
{
    /// <summary>
    /// Starts the worker. Called once when the host starts.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gracefully stops the worker. Called once when the host is shutting down.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
