namespace LHA.BackgroundWorker;

/// <summary>
/// Global configuration for the background worker subsystem.
/// </summary>
public sealed class BackgroundWorkerOptions
{
    /// <summary>
    /// Master switch to enable/disable all background workers.
    /// Default: <see langword="true"/>.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Grace period for workers to complete during shutdown.
    /// Default: 30 seconds.
    /// </summary>
    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
