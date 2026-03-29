using LHA.TenantManagement.Domain;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.Account.Application.TenantProvisioning;

/// <summary>
/// A strategy implementation to provision databases based on CMultiTenancyDatabaseStyle.
/// </summary>
public interface ITenantProvisionerStrategy
{
    /// <summary>
    /// The database style this strategy applies to.
    /// </summary>
    CMultiTenancyDatabaseStyle Style { get; }

    /// <summary>
    /// Dispatches logic to create schemas or databases specific to the tenant.
    /// </summary>
    Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
}
