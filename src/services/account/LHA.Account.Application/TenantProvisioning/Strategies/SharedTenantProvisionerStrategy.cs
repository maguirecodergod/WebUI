using LHA.TenantManagement.Domain;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.Account.Application.TenantProvisioning.Strategies;

/// <summary>
/// Provisioning strategy for tenants that share the default database setup.
/// </summary>
public sealed class SharedTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    public CMultiTenancyDatabaseStyle Style => CMultiTenancyDatabaseStyle.Shared;

    public Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        // For shared databases, they use the Host's existing connection string.
        // There's no isolated physical database to provision.
        return Task.CompletedTask;
    }
}
