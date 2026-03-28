using LHA.Core;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IIdentityRoleRepository"/>.
/// </summary>
public sealed class EfCoreIdentityRoleRepository
    : EfCoreRepository<IdentityDbContext, IdentityRole, Guid>, IIdentityRoleRepository
{
    private static readonly string[] SearchColumns = ["Name"];

    public EfCoreIdentityRoleRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <inheritdoc />
    public override async Task<IdentityRole?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(r => r.Claims)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IdentityRole?> FindByNormalizedNameAsync(
        string normalizedName, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(r => r.Claims)
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityRole>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Include(r => r.Claims)
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, r => r.Status == status!.Value)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        string? filter, CMasterStatus? status,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, r => r.Status == status!.Value)
            .LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityRole>> GetDefaultRolesAsync(CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(r => r.IsDefault && r.Status == CMasterStatus.Active)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityRole>> GetListByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();
        if (idList.Count == 0) return [];

        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(r => idList.Contains(r.Id))
            .ToListAsync(cancellationToken);
    }
}
