namespace LHA.MultiTenancy;

/// <summary>
/// Resolves connection strings for the current tenant.
/// Falls back to the host connection string when the tenant
/// does not define an override for the requested name.
/// </summary>
public sealed class MultiTenantConnectionStringResolver
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantStore _tenantStore;

    public MultiTenantConnectionStringResolver(ICurrentTenant currentTenant, ITenantStore tenantStore)
    {
        ArgumentNullException.ThrowIfNull(currentTenant);
        ArgumentNullException.ThrowIfNull(tenantStore);
        _currentTenant = currentTenant;
        _tenantStore = tenantStore;
    }

    /// <summary>
    /// Returns the connection string for the current tenant and given name,
    /// or <c>null</c> if no matching override is configured.
    /// </summary>
    /// <param name="connectionStringName">Logical name (e.g., "Default").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<string?> ResolveAsync(
        string connectionStringName = "Default",
        CancellationToken cancellationToken = default)
    {
        if (!_currentTenant.IsAvailable)
        {
            return null; // Host context — caller should use its own default
        }

        var tenant = await _tenantStore.FindAsync(_currentTenant.Id!.Value, cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        return tenant.ConnectionStrings.TryGetValue(connectionStringName, out var cs) ? cs : null;
    }
}
