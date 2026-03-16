namespace LHA.MultiTenancy;

/// <summary>
/// Circuit breaker state for a tenant.
/// Transitions: Closed → Open → HalfOpen → Closed (or back to Open).
/// </summary>
public enum CTenantCircuitStateType
{
    /// <summary>Normal operation; all requests pass through.</summary>
    Closed = 1,

    /// <summary>Failures exceeded threshold; requests are rejected immediately.</summary>
    Open = 2,

    /// <summary>Recovery probe; a limited number of requests are allowed through.</summary>
    HalfOpen = 3
}
