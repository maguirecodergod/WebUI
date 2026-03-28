using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.Account.EntityFrameworkCore.Repositories;

public class EfCoreEntityPropertyChangeRepository
    : EfCoreRepository<AccountDbContext, EntityPropertyChangeEntity, Guid>, IEntityPropertyChangeRepository
{
    public EfCoreEntityPropertyChangeRepository(IDbContextProvider<AccountDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<List<EntityPropertyChangeEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        Guid? entityChangeId = null,
        string? propertyName = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, entityChangeId, propertyName);

        return await query
            .SortByDynamic(sorter, nameof(EntityPropertyChangeEntity.PropertyName), true)
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? entityChangeId = null,
        string? propertyName = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, entityChangeId, propertyName);

        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<EntityPropertyChangeEntity> ApplyFilter(
        IQueryable<EntityPropertyChangeEntity> query,
        Guid? entityChangeId = null,
        string? propertyName = null)
    {
        return query
            .WhereIf(entityChangeId.HasValue, x => x.EntityChangeId == entityChangeId)
            .WhereIf(!string.IsNullOrWhiteSpace(propertyName), x => x.PropertyName.Contains(propertyName!));
    }
}
