using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.EntityChangeProperties;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.EntityPropertyChanges;
using LHA.Ddd.Application;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// MongoDB implementation of <see cref="IEntityPropertyChangeRepository"/>.
/// </summary>
public class EfCoreEntityPropertyChangeMongoDbRepository
    : EfCoreRepository<AuditLogDbContext, EntityPropertyChangeEntity, Guid>, IEntityPropertyChangeRepository
{
    public EfCoreEntityPropertyChangeMongoDbRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<PagedResultDto<EntityPropertyChangeEntity>> GetWithPaginationAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking();
        query = ApplyFilter(query, request);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .SortByDynamic(request.Sorter, nameof(EntityPropertyChangeEntity.Id), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<EntityPropertyChangeEntity>(
            totalCount,
            items,
            request.PageNumber,
            request.PageSize);
    }

    public virtual async Task<List<EntityPropertyChangeEntity>> GetListAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();
        query = query.AsNoTracking();
        query = ApplyFilter(query, request);

        return await query
            .SortByDynamic(request.Sorter, nameof(EntityPropertyChangeEntity.Id), defaultAscending: true)
            .PageBy(request)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EntityPropertyChangeEntity> query = await GetDbSetAsync();
        query = ApplyFilter(query, request);
        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<EntityPropertyChangeEntity> ApplyFilter(
        IQueryable<EntityPropertyChangeEntity> query,
        EntityPropertyChangePagedRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            string lowerSearchQuery = request.SearchQuery.ToLowerInvariant();
            string[] allowSearchColumns = request.AllowSearchColumns.Any() ? request.AllowSearchColumns
                : new string[] { nameof(EntityPropertyChangeEntity.PropertyName), nameof(EntityPropertyChangeEntity.PropertyTypeFullName) };

            query = query.SearchDynamic(lowerSearchQuery, allowSearchColumns, Core.CSearchOperatorType.Contains, true, Core.CSearchCombineModeType.Or);
        }

        return query;
    }
}
