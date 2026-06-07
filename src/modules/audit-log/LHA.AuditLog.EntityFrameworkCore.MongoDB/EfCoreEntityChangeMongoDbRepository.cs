using LHA.AuditLog.Domain;
using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.EntityChanges;
using LHA.Ddd.Application;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// MongoDB implementation of <see cref="IEntityChangeRepository"/>.
/// </summary>
public class EfCoreEntityChangeMongoDbRepository
    : EfCoreRepository<AuditLogDbContext, EntityChangeEntity, Guid>,
    IEntityChangeRepository
{
    public EfCoreEntityChangeMongoDbRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<PagedResultDto<EntityChangeEntity>> GetWithPaginationAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking();
        query = ApplyFilter(query, request);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Include(x => x.PropertyChanges)
            .SortByDynamic(request.Sorter, nameof(EntityChangeEntity.ChangeTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<EntityChangeEntity>(
            totalCount,
            items,
            request.PageNumber,
            request.PageSize);
    }

    public virtual async Task<List<EntityChangeEntity>> GetListAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityChangeEntity> query = await GetDbSetAsync();
        query = query.AsNoTracking().Include(x => x.PropertyChanges);
        query = ApplyFilter(query, request);

        return await query
            .SortByDynamic(request.Sorter, nameof(EntityChangeEntity.ChangeTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityChangeEntity> query = await GetDbSetAsync();
        query = ApplyFilter(query, request);
        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<EntityChangeEntity> ApplyFilter(
        IQueryable<EntityChangeEntity> query,
        EntityChangePagedRequest input)
    {
        if (!string.IsNullOrWhiteSpace(input.SearchQuery))
        {
            string searchLower = input.SearchQuery.ToLowerInvariant();
            string[] filterSearchColumns = input.AllowSearchColumns?.Length > 0 ? input.AllowSearchColumns
                : new[] { nameof(EntityChangeEntity.EntityId), nameof(EntityChangeEntity.EntityTypeFullName) };

            query = query.SearchDynamic(keyword: searchLower,
                searchColumns: filterSearchColumns,
                searchOperator: Core.CSearchOperatorType.Contains,
                ignoreCase: true,
                combineMode: Core.CSearchCombineModeType.Or);
        }

        if (input.Filter != null)
        {
            query = query.WhereIf(input.Filter.ChangeTypes != null && input.Filter.ChangeTypes.Any(),
                predicate: x => (input.Filter.ChangeTypes ?? new List<CEntityChangeType>()).Contains(x.ChangeType));
        }

        return query;
    }
}