namespace LHA.MultiTenancy.Provisioning;

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

    public async Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, int style, CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.FirstOrDefault(s => s.Style == style);
        
        if (strategy == null)
            throw new NotSupportedException($"Provisioning strategy for DatabaseStyle '{style}' is not implemented.");

        return await strategy.ProvisionAsync(tenantId, normalizedTenantName, cancellationToken);
    }
}
