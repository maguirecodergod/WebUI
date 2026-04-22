namespace LHA.MultiTenancy.Provisioning;

/// <summary>
/// Orchestrates the process of provisioning a tenant's infrastructure
/// (e.g. database creation) after a tenant record gets saved.
/// </summary>
public interface ITenantProvisioningOrchestrator
{
    /// <summary>
    /// Evaluates the tenant's DatabaseStyle and delegates to the appropriate Strategy.
    /// Returns the updated connection string if applicable.
    /// </summary>
    Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, int style, CancellationToken cancellationToken = default);
}
