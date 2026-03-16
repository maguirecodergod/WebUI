namespace LHA.MultiTenancy;

/// <summary>
/// Null-object tenant store that returns no tenants.
/// Used as the default before a real store is registered.
/// </summary>
internal sealed class NullTenantStore : ITenantStore
{
    public Task<TenantConfiguration?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult<TenantConfiguration?>(null);

    public Task<TenantConfiguration?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        => Task.FromResult<TenantConfiguration?>(null);

    public Task<IReadOnlyList<TenantConfiguration>> GetListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TenantConfiguration>>([]);
}
