using System.Threading;
using System.Threading.Tasks;
using LHA.TenantManagement.Domain;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.Account.Application.TenantProvisioning.Strategies;

/// <summary>
/// Provisioning strategy for Hybrid isolation schemas.
/// Could dynamically resolve shared or isolated databases, but defaulting to Host pattern for v1.
/// </summary>
public sealed class HybridTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    public CMultiTenancyDatabaseStyle Style => CMultiTenancyDatabaseStyle.Hybrid;

    public Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        // Implement advanced custom logic here
        // (Like finding least loaded database shards or creating one selectively).
        return Task.CompletedTask;
    }
}
