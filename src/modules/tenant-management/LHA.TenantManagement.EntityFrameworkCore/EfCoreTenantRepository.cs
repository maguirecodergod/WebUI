using LHA.Core;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="ITenantRepository"/>.
/// Extends the generic <see cref="EfCoreRepository{TDbContext,TEntity,TKey}"/>
/// with tenant-specific queries including eager-loaded connection strings.
/// <para>
/// Uses the framework-level <see cref="QueryableExtensions.SearchDynamic{T}"/>
/// and <see cref="QueryableExtensions.SortByDynamic{T}"/> for dynamic filtering
/// and sorting — no hardcoded switch/case required.
/// </para>
/// </summary>
public sealed class EfCoreTenantRepository
    : EfCoreRepository<TenantManagementDbContext, TenantEntity, Guid>, ITenantRepository
{
    /// <summary>
    /// Columns that are searchable by the keyword filter.
    /// </summary>
    private static readonly string[] SearchColumns = ["Name", "NormalizedName"];

    public EfCoreTenantRepository(IDbContextProvider<TenantManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public override async Task<TenantEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(t => t.ConnectionStrings)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TenantEntity?> FindByNameAsync(
        string normalizedName,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(t => t.ConnectionStrings)
            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<TenantEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .Include(t => t.ConnectionStrings)
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, t => t.Status == status!.Value)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(status.HasValue, t => t.Status == status!.Value)
            .LongCountAsync(cancellationToken);
    }
}
