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
        string? filter, string? sorting, int skipCount, int maxResultCount,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .SortByDynamic(sorting, defaultProperty: "Name")
            .Skip(skipCount)
            .Take(maxResultCount)
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
