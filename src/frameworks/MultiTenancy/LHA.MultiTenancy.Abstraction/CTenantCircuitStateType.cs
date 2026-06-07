namespace LHA.MultiTenancy;

/// <summary>
/// Circuit breaker state for a tenant.
/// Transitions: Closed → Open → HalfOpen → Closed (or back to Open).
/// </summary>
public enum CTenantCircuitStateType
{
    /// <summary>
    /// 1 - Closed: Normal operation; all requests pass through.
    /// </summary>
    Closed = 1,

    /// <summary>
    /// 2 - Open: Failures exceeded threshold; requests are rejected immediately.
    /// </summary>
    Open = 2,

    /// <summary>
    /// 3 - HalfOpen: Recovery probe; a limited number of requests are allowed through.
    /// </summary>
    HalfOpen = 3
}
