using LHA.UnitOfWork;
using LHA.AuditLog.Domain;
using LHA.Ddd.Application;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LHA.Shared.Domain.AuditLogActions;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// PostgreSQL implementation of <see cref="IAuditLogActionRepository"/>.
/// </summary>
public class EfCoreAuditLogActionPostgreSqlRepository
    : EfCoreRepository<AuditLogDbContext, AuditLogActionEntity, Guid>, IAuditLogActionRepository
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public EfCoreAuditLogActionPostgreSqlRepository(
        IDbContextProvider<AuditLogDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// Get audit log actions with pagination.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<PagedResultDto<AuditLogActionEntity>> GetWithPaginationAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var getListQuery = dbContext.Set<AuditLogActionEntity>().AsNoTracking();
        getListQuery = ApplyFilter(getListQuery, request);

        var countQuery = dbContext.Set<AuditLogActionEntity>().AsNoTracking();
        countQuery = ApplyFilter(countQuery, request);

        var totalCount = await countQuery.LongCountAsync(cancellationToken);

        var list = await getListQuery
            .SortByDynamic(request.Sorter, nameof(AuditLogEntity.ExecutionTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AuditLogActionEntity>(
            totalCount,
            list,
            request.PageNumber,
            request.PageSize);
    }

    /// <summary>
    /// Get audit log actions list.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<List<AuditLogActionEntity>> GetListAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogActionEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, request);

        return await query
            .SortByDynamic(request.Sorter, nameof(AuditLogActionEntity.ExecutionTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get audit log actions count.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<long> GetCountAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogActionEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, request);

        return await query.LongCountAsync(cancellationToken);
    }

    /// <summary>
    /// Get audit log actions list by audit log id.
    /// </summary>
    /// <param name="auditLogId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<List<AuditLogActionEntity>> GetByAuditLogIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .Where(x => x.AuditLogId == auditLogId)
            .OrderBy(x => x.ExecutionTime)
            .ToListAsync(cancellationToken);
    }

    protected virtual IQueryable<AuditLogActionEntity> ApplyFilter(
        IQueryable<AuditLogActionEntity> query,
        AuditLogActionPagedRequest request)
    {
        if (request.AuditLogId.HasValue)
        {
            query = query.Where(x => x.AuditLogId == request.AuditLogId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            string searchLower = request.SearchQuery.ToLowerInvariant();
            var filterSearchColumns = request.AllowSearchColumns?.Length > 0 ? request.AllowSearchColumns
                : new[] { nameof(AuditLogActionEntity.MethodName), nameof(AuditLogActionEntity.ServiceName) };
            query = query.SearchDynamic(keyword: searchLower,
                searchColumns: filterSearchColumns,
                searchOperator: Core.CSearchOperatorType.Contains,
                ignoreCase: true, combineMode: Core.CSearchCombineModeType.Or);
        }

        if (request.Filter != null)
        {
            if (request.Filter.ExecutionStartTime.HasValue)
            {
                query = query.Where(
                    x => x.ExecutionTime >= request.Filter.ExecutionStartTime.Value);
            }

            if (request.Filter.ExecutionEndTime.HasValue)
            {
                query = query.Where(
                    x => x.ExecutionTime <= request.Filter.ExecutionEndTime.Value);
            }

            if (request.Filter.MinExecutionDuration.HasValue)
            {
                query = query.Where(
                    x => x.ExecutionDuration >= request.Filter.MinExecutionDuration.Value);
            }

            if (request.Filter.MaxExecutionDuration.HasValue)
            {
                query = query.Where(
                    x => x.ExecutionDuration <= request.Filter.MaxExecutionDuration.Value);
            }
        }

        return query;
    }
}
