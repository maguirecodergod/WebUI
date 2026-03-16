namespace LHA.MultiTenancy;

/// <summary>
/// Per-tenant circuit breaker that prevents cascading failures.
/// Each tenant's circuit is tracked independently.
/// </summary>
public interface ITenantCircuitBreaker
{
    /// <summary>
    /// Gets the current circuit state for the specified tenant.
    /// </summary>
    CTenantCircuitStateType GetState(Guid tenantId);

    /// <summary>
    /// Records a successful operation for the tenant, potentially closing the circuit.
    /// </summary>
    void RecordSuccess(Guid tenantId);

    /// <summary>
    /// Records a failed operation for the tenant, potentially opening the circuit.
    /// </summary>
    void RecordFailure(Guid tenantId);

    /// <summary>
    /// Checks whether requests for the given tenant should be allowed.
    /// Returns <see langword="false"/> when the circuit is open.
    /// </summary>
    bool AllowRequest(Guid tenantId);

    /// <summary>
    /// Manually resets the circuit for a tenant (e.g., after admin intervention).
    /// </summary>
    void Reset(Guid tenantId);
}
