namespace LHA.BackgroundJob;

/// <summary>
/// Configuration options for the default store-based background job worker.
/// </summary>
public sealed class BackgroundJobWorkerOptions
{
    /// <summary>
    /// Application name for multi-app job isolation.
    /// Only jobs matching this name are processed.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// Interval between polling the job store for waiting jobs (milliseconds).
    /// Default: 5000 (5 seconds).
    /// </summary>
    public int JobPollPeriodMs { get; set; } = 5000;

    /// <summary>
    /// Maximum number of jobs to fetch from the store per poll cycle.
    /// Default: 1000.
    /// </summary>
    public int MaxJobFetchCount { get; set; } = 1000;

    /// <summary>
    /// Wait duration (seconds) before the first retry after a failure.
    /// Default: 60 (1 minute).
    /// </summary>
    public int FirstRetryWaitSeconds { get; set; } = 60;

    /// <summary>
    /// Timeout (seconds) from job creation. If exceeded, the job is abandoned.
    /// Default: 172800 (2 days).
    /// </summary>
    public int JobTimeoutSeconds { get; set; } = 172800;

    /// <summary>
    /// Wait factor multiplied by the previous wait time for exponential backoff.
    /// Default: 2.0.
    /// </summary>
    public double RetryWaitFactor { get; set; } = 2.0;

    /// <summary>
    /// Distributed lock name used by the worker to ensure single-node execution.
    /// Default: "LHABackgroundJobWorker".
    /// </summary>
    public string DistributedLockName { get; set; } = "LHABackgroundJobWorker";

    /// <summary>
    /// If no distributed lock can be acquired, the worker waits this multiplier
    /// of <see cref="JobPollPeriodMs"/> before retrying.
    /// Default: 12 (i.e. 60 seconds at default poll rate).
    /// </summary>
    public int LockWaitMultiplier { get; set; } = 12;
}
