namespace LHA.MultiTenancy;

/// <summary>
/// Configuration for the tenant-level circuit breaker.
/// </summary>
public sealed class TenantCircuitBreakerOptions
{
    /// <summary>
    /// Whether tenant-level circuit breaking is enabled.
    /// Default: <see langword="true"/>.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Number of consecutive failures before the circuit opens.
    /// Default: 5.
    /// </summary>
    public int FailureThreshold { get; set; } = 5;

    /// <summary>
    /// Duration the circuit stays open before transitioning to half-open.
    /// Default: 30 seconds.
    /// </summary>
    public TimeSpan OpenDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Number of probe requests allowed in half-open state.
    /// Default: 1.
    /// </summary>
    public int HalfOpenMaxProbes { get; set; } = 1;

    /// <summary>
    /// Number of consecutive successes in half-open state required to close the circuit.
    /// Default: 2.
    /// </summary>
    public int SuccessThresholdToClose { get; set; } = 2;

    /// <summary>
    /// Time window for counting failures. Failures outside this window are ignored.
    /// Default: 60 seconds.
    /// </summary>
    public TimeSpan FailureWindow { get; set; } = TimeSpan.FromSeconds(60);
}
