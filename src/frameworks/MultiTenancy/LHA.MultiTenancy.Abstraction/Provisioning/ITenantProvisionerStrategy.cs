namespace LHA.MultiTenancy.Provisioning;

/// <summary>
/// A strategy implementation to provision databases based on database style.
/// </summary>
public interface ITenantProvisionerStrategy
{
    /// <summary>
    /// The database style this strategy applies to (e.g. 0 = Shared, 1 = PerTenant, 2 = PerSchema).
    /// </summary>
    int Style { get; }

    /// <summary>
    /// Dispatches logic to create schemas or databases specific to the tenant.
    /// Returns the connection string if it was modified/generated.
    /// </summary>
    Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, CancellationToken cancellationToken = default);
}
