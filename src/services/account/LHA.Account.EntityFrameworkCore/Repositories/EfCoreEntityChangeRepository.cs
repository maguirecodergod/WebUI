using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.Account.EntityFrameworkCore.Repositories;

public class EfCoreEntityChangeRepository
    : EfCoreRepository<AccountDbContext, EntityChangeEntity, Guid>, IEntityChangeRepository
{
    public EfCoreEntityChangeRepository(IDbContextProvider<AccountDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<List<EntityChangeEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        Guid? auditLogId = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CEntityChangeType? changeType = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(includeDetails);

        query = ApplyFilter(query, auditLogId, entityTypeFullName, entityId, changeType);

        return await query
            .SortByDynamic(sorter, nameof(EntityChangeEntity.ChangeTime), false)
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? auditLogId = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CEntityChangeType? changeType = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityChangeEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, auditLogId, entityTypeFullName, entityId, changeType);

        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<EntityChangeEntity> ApplyFilter(
        IQueryable<EntityChangeEntity> query,
        Guid? auditLogId = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CEntityChangeType? changeType = null)
    {
        return query
            .WhereIf(auditLogId.HasValue, x => x.AuditLogId == auditLogId)
            .WhereIf(!string.IsNullOrWhiteSpace(entityTypeFullName), x => x.EntityTypeFullName != null && x.EntityTypeFullName.Contains(entityTypeFullName!))
            .WhereIf(!string.IsNullOrWhiteSpace(entityId), x => x.EntityId == entityId)
            .WhereIf(changeType.HasValue, x => x.ChangeType == changeType);
    }

    protected virtual async Task<IQueryable<EntityChangeEntity>> GetQueryableAsync(bool includeDetails = false)
    {
        var query = await GetDbSetAsync();

        if (includeDetails)
        {
            return query.Include(x => x.PropertyChanges);
        }

        return query;
    }
}
