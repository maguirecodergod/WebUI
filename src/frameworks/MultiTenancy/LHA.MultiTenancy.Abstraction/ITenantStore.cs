namespace LHA.MultiTenancy;

/// <summary>
/// Persistence abstraction for tenant configurations.
/// Implementations may read from database, configuration files, or remote APIs.
/// </summary>
public interface ITenantStore
{
    Task<TenantConfiguration?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TenantConfiguration?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TenantConfiguration>> GetListAsync(CancellationToken cancellationToken = default);
}
