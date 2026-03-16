namespace LHA.DistributedLocking;

/// <summary>
/// Health check for the distributed lock infrastructure.
/// </summary>
public interface IDistributedLockHealthCheck
{
    /// <summary>
    /// Verifies the distributed lock service is operational
    /// by attempting to acquire and release a probe lock.
    /// </summary>
    Task<DistributedLockHealthResult> CheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a distributed lock health check.
/// </summary>
/// <param name="IsHealthy">Whether the lock provider is available and functional.</param>
/// <param name="Description">A human-readable description of the health state.</param>
/// <param name="ElapsedMs">Time taken for the health check in milliseconds.</param>
public sealed record DistributedLockHealthResult(
    bool IsHealthy,
    string Description,
    double ElapsedMs);
