namespace LHA.MultiTenancy.Provisioning.Strategies;

/// <summary>
/// Provisioning strategy for tenants that share the default database setup.
/// </summary>
public sealed class SharedTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    public int Style => 1; // Shared

    public Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, CancellationToken cancellationToken = default)
    {
        // For shared databases, they use the Host's existing connection string.
        // There's no isolated physical database to provision.
        return Task.FromResult<string?>(null);
    }
}
