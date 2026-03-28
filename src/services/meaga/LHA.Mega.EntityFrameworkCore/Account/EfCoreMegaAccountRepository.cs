using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.Mega.Domain.Account;
using Microsoft.EntityFrameworkCore;

namespace LHA.Mega.EntityFrameworkCore.Account;

public sealed class EfCoreMegaAccountRepository
    : EfCoreRepository<MegaDbContext, MegaAccountEntity, Guid>, IMegaAccountRepository
{
    public EfCoreMegaAccountRepository(IDbContextProvider<MegaDbContext> dbContextProvider)
        : base(dbContextProvider) { }

    public async Task<MegaAccountEntity?> FindByCodeAsync(
        string code, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(a => a.Code == code, cancellationToken);
    }

    public async Task<List<MegaAccountEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await ApplyFilter(dbSet, filter, isActive)
            .SortByDynamic(sorter, defaultProperty: nameof(MegaAccountEntity.Code))
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(
        string? filter, bool? isActive, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await ApplyFilter(dbSet, filter, isActive).LongCountAsync(cancellationToken);
    }

    private static IQueryable<MegaAccountEntity> ApplyFilter(
        IQueryable<MegaAccountEntity> query, string? filter, bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(a =>
                EF.Functions.ILike(a.Code, $"%{f}%") ||
                EF.Functions.ILike(a.Name, $"%{f}%"));
        }

        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        return query;
    }
}
