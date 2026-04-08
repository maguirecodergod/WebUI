using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IPermissionGrantRepository"/>.
/// </summary>
public sealed class EfCorePermissionGrantRepository
    : EfCoreRepository<IdentityDbContext, IdentityPermissionGrant, Guid>, IPermissionGrantRepository
{
    private readonly ICurrentTenant _currentTenant;

    public EfCorePermissionGrantRepository(
        IDbContextProvider<IdentityDbContext> dbContextProvider,
        ICurrentTenant currentTenant)
        : base(dbContextProvider)
    {
        _currentTenant = currentTenant;
    }

    /// <inheritdoc />
    public async Task<IdentityPermissionGrant?> FindAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                pg => (pg.TenantId == _currentTenant.Id || pg.TenantId == null) &&
                      pg.Name == name &&
                      pg.ProviderName == providerName &&
                      pg.ProviderKey == providerKey,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityPermissionGrant>> GetListAsync(
        string providerName, string providerKey,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.IgnoreQueryFilters()
            .Where(pg => (pg.TenantId == _currentTenant.Id || pg.TenantId == null) &&
                         pg.ProviderName == providerName && 
                         pg.ProviderKey == providerKey)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityPermissionGrant>> GetListByNameAsync(
        string name, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.IgnoreQueryFilters()
            .Where(pg => (pg.TenantId == _currentTenant.Id || pg.TenantId == null) &&
                         pg.Name == name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        await dbSet.IgnoreQueryFilters()
            .Where(pg => (pg.TenantId == _currentTenant.Id || pg.TenantId == null) &&
                         pg.Name == name &&
                         pg.ProviderName == providerName &&
                         pg.ProviderKey == providerKey)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityPermissionGrant>> GetGrantsByProvidersAsync(
        string providerName, IReadOnlyCollection<string> providerKeys,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.IgnoreQueryFilters()
            .Where(pg => (pg.TenantId == _currentTenant.Id || pg.TenantId == null) &&
                         pg.ProviderName == providerName && 
                         providerKeys.Contains(pg.ProviderKey))
            .ToListAsync(cancellationToken);
    }
}
