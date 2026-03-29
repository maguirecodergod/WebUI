using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LHA.TenantManagement.Domain;

namespace LHA.Account.Application.TenantProvisioning;

/// <summary>
/// Orchestrates the process of provisioning by locating the correct strategy
/// based on the tenant's DatabaseStyle.
/// </summary>
public sealed class TenantProvisioningOrchestrator : ITenantProvisioningOrchestrator
{
    private readonly IEnumerable<ITenantProvisionerStrategy> _strategies;

    public TenantProvisioningOrchestrator(IEnumerable<ITenantProvisionerStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Style == tenant.DatabaseStyle);
        
        if (strategy == null)
            throw new NotSupportedException($"Provisioning strategy for DatabaseStyle '{tenant.DatabaseStyle}' is not implemented.");

        await strategy.ProvisionAsync(tenant, cancellationToken);
    }
}
