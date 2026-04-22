namespace LHA.MultiTenancy.Provisioning.Strategies;

/// <summary>
/// Provisioning strategy for Hybrid isolation schemas.
/// Could dynamically resolve shared or isolated databases, but defaulting to Host pattern for v1.
/// </summary>
public sealed class HybridTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    public int Style => 7; // Hybrid

    public Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, CancellationToken cancellationToken = default)
    {
        // Implement advanced custom logic here
        // (Like finding least loaded database shards or creating one selectively).
        return Task.FromResult<string?>(null);
    }
}
