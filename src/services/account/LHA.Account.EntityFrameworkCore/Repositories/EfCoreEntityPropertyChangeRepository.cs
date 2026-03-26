using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
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
        Guid? entityChangeId = null,
        string? propertyName = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, entityChangeId, propertyName);

        return await query
            .SortByDynamic(sorting, nameof(EntityPropertyChangeEntity.PropertyName), true)
            .Skip(skipCount)
            .Take(maxResultCount)
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
