namespace LHA.Scheduling;

/// <summary>
/// Health check abstraction for scheduler infrastructure.
/// Register implementations in DI and use with ASP.NET Core health checks.
/// </summary>
public interface ISchedulingHealthCheck
{
    /// <summary>
    /// Checks the health of the scheduler infrastructure (storage, workers, connections).
    /// </summary>
    Task<SchedulerHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>The scheduler type identifier (e.g. "Hangfire", "Quartz").</summary>
    string SchedulerType { get; }
}

/// <summary>
/// Result of a scheduler health check.
/// </summary>
public sealed record SchedulerHealthResult
{
    /// <summary>Whether the scheduler infrastructure is healthy.</summary>
    public required bool IsHealthy { get; init; }

    /// <summary>Human-readable status description.</summary>
    public string? Description { get; init; }

    /// <summary>Additional diagnostic data (e.g. pending job count, worker count).</summary>
    public IReadOnlyDictionary<string, object> Data { get; init; } = new Dictionary<string, object>();

    /// <summary>Exception if the check failed.</summary>
    public Exception? Exception { get; init; }

    public static SchedulerHealthResult Healthy(string? description = null)
        => new() { IsHealthy = true, Description = description ?? "Scheduler is healthy" };

    public static SchedulerHealthResult Unhealthy(string description, Exception? exception = null)
        => new() { IsHealthy = false, Description = description, Exception = exception };
}
