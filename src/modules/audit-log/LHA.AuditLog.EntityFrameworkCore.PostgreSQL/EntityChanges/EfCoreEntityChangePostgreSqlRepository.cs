using LHA.AuditLog.Domain;
using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.EntityChanges;
using LHA.Ddd.Application;
using Microsoft.EntityFrameworkCore;
using LHA.UnitOfWork;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// PostgreSQL implementation of <see cref="IEntityChangeRepository"/>.
/// </summary>
public class EfCoreEntityChangePostgreSqlRepository
    : EfCoreRepository<AuditLogDbContext, EntityChangeEntity, Guid>,
    IEntityChangeRepository
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public EfCoreEntityChangePostgreSqlRepository(
        IDbContextProvider<AuditLogDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }


    public virtual async Task<PagedResultDto<EntityChangeEntity>> GetWithPaginationAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query1 = dbContext.Set<EntityChangeEntity>().AsNoTracking();
        query1 = ApplyFilter(query1, request);

        var query2 = dbContext.Set<EntityChangeEntity>().AsNoTracking();
        query2 = ApplyFilter(query2, request);

        var totalCount = await query2.LongCountAsync(cancellationToken);

        var list = await query1
            .Include(x => x.PropertyChanges)
            .SortByDynamic(request.Sorter, nameof(EntityChangeEntity.ChangeTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<EntityChangeEntity>(
            totalCount,
            list,
            request.PageNumber,
            request.PageSize);
    }


    public virtual async Task<List<EntityChangeEntity>> GetListAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(includeDetails: true);

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

    protected virtual async Task<IQueryable<EntityChangeEntity>> GetQueryableAsync(bool includeDetails = false)
    {
        var query = await GetDbSetAsync();
        return includeDetails ? query.Include(x => x.PropertyChanges) : query;
    }
}
