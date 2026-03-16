using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IPermissionGrantRepository"/>.
/// </summary>
public sealed class EfCorePermissionGrantRepository
    : EfCoreRepository<IdentityDbContext, IdentityPermissionGrant, Guid>, IPermissionGrantRepository
{
    public EfCorePermissionGrantRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <inheritdoc />
    public async Task<IdentityPermissionGrant?> FindAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(
            pg => pg.Name == name &&
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
        return await dbSet
            .Where(pg => pg.ProviderName == providerName && pg.ProviderKey == providerKey)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityPermissionGrant>> GetListByNameAsync(
        string name, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(pg => pg.Name == name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string name, string providerName, string providerKey,
        CancellationToken cancellationToken)
    {
        var dbContext = await GetDbContextAsync();
        await dbContext.Set<IdentityPermissionGrant>()
            .Where(pg => pg.Name == name &&
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
        return await dbSet
            .Where(pg => pg.ProviderName == providerName && providerKeys.Contains(pg.ProviderKey))
            .ToListAsync(cancellationToken);
    }
}
