using LHA.TenantManagement.Domain;

namespace LHA.Account.Application.TenantProvisioning;

/// <summary>
/// Orchestrates the process of provisioning a tenant's infrastructure
/// (e.g. database creation) after a tenant record gets saved.
/// </summary>
public interface ITenantProvisioningOrchestrator
{
    /// <summary>
    /// Evaluates the tenant's DatabaseStyle and delegates to the appropriate Strategy.
    /// </summary>
    Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
}
