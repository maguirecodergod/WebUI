using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IIdentityClaimTypeRepository"/>.
/// </summary>
public sealed class EfCoreIdentityClaimTypeRepository
    : EfCoreRepository<IdentityDbContext, IdentityClaimType, Guid>, IIdentityClaimTypeRepository
{
    private static readonly string[] SearchColumns = ["Name", "Description"];

    public EfCoreIdentityClaimTypeRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <inheritdoc />
    public async Task<bool> AnyAsync(string name, Guid? excludeId, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet
            .WhereIf(excludeId.HasValue, ct => ct.Id != excludeId!.Value)
            .AnyAsync(ct => ct.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<IdentityClaimType>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(string? filter, CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .LongCountAsync(cancellationToken);
    }
}
