using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IIdentitySecurityLogRepository"/>.
/// </summary>
public sealed class EfCoreIdentitySecurityLogRepository
    : EfCoreRepository<IdentityDbContext, IdentitySecurityLog, Guid>, IIdentitySecurityLogRepository
{
    private static readonly string[] SearchColumns = ["Action", "UserName", "ClientIpAddress"];

    public EfCoreIdentitySecurityLogRepository(IDbContextProvider<IdentityDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    /// <inheritdoc />
    public async Task<List<IdentitySecurityLog>> GetListAsync(
        string? filter, Guid? userId, string? action,
        string? sorting, int skipCount, int maxResultCount,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(userId.HasValue, sl => sl.UserId == userId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(action), sl => sl.Action == action)
            .SortByDynamic(sorting, defaultProperty: "CreationTime", defaultAscending: false)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        string? filter, Guid? userId, string? action,
        CancellationToken cancellationToken)
    {
        var dbSet = await GetDbSetAsync();

        return await dbSet.AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(userId.HasValue, sl => sl.UserId == userId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(action), sl => sl.Action == action)
            .LongCountAsync(cancellationToken);
    }
}
