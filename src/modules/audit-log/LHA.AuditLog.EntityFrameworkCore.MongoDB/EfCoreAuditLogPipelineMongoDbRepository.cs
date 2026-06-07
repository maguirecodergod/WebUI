using LHA.AuditLog.Domain;
using LHA.Core;
using LHA.Ddd.Application;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.AuditLogPipelines;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// MongoDB implementation of <see cref="IAuditLogPipelineRepository"/>.
/// </summary>
public class EfCoreAuditLogPipelineMongoDbRepository
    : EfCoreRepository<AuditLogDbContext, AuditLogPipelineEntity, Guid>, IAuditLogPipelineRepository
{
    public EfCoreAuditLogPipelineMongoDbRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Gets pipeline audit logs with pagination and filtering.
    /// </summary>
    public virtual async Task<PagedResultDto<AuditLogPipelineEntity>> GetWithPaginationAsync(
        AuditLogPipelinePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet.AsNoTracking();
        query = ApplyFilter(query, request);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .SortByDynamic(request.Sorter, nameof(AuditLogPipelineEntity.Timestamp), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AuditLogPipelineEntity>(
            totalCount,
            items,
            request.PageNumber,
            request.PageSize);
    }

    /// <summary>
    /// Gets a list of pipeline audit logs matching the criteria.
    /// </summary>
    public virtual async Task<List<AuditLogPipelineEntity>> GetListAsync(
        AuditLogPipelinePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogPipelineEntity> query = await GetDbSetAsync();
        query = query.AsNoTracking();
        query = ApplyFilter(query, request);

        return await query
            .SortByDynamic(request.Sorter, nameof(AuditLogPipelineEntity.Timestamp), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        AuditLogPipelinePagedRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogPipelineEntity> query = await GetDbSetAsync();
        query = query.AsNoTracking();
        query = ApplyFilter(query, request);

        return await query.LongCountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets pipeline audit logs by correlation ID.
    /// </summary>
    public virtual async Task<List<AuditLogPipelineEntity>> GetByCorrelationIdAsync(
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .Where(x => x.CorrelationId == correlationId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets pipeline audit logs by trace ID.
    /// </summary>
    public virtual async Task<List<AuditLogPipelineEntity>> GetByTraceIdAsync(
        string traceId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .Where(x => x.TraceId == traceId)
            .OrderByDescending(x => x.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets recent pipeline audit logs by user ID.
    /// </summary>
    public virtual async Task<List<AuditLogPipelineEntity>> GetByUserIdAsync(
        string userId,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes old pipeline audit logs before the specified cutoff time.
    /// </summary>
    public virtual async Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var logsToDelete = await dbContext.Set<AuditLogPipelineEntity>()
            .Where(x => x.Timestamp < cutoffTime)
            .ToListAsync(cancellationToken);

        if (logsToDelete.Count == 0) return 0;

        dbContext.Set<AuditLogPipelineEntity>().RemoveRange(logsToDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        return logsToDelete.Count;
    }

    /// <summary>
    /// Applies filters to the query based on the request criteria.
    /// </summary>
    protected virtual IQueryable<AuditLogPipelineEntity> ApplyFilter(
        IQueryable<AuditLogPipelineEntity> query,
        AuditLogPipelinePagedRequest request)
    {
        var filter = request.Filter;

        if (filter != null)
        {
            query = query.WhereIf(filter.TimestampStart.HasValue, x => x.Timestamp >= filter.TimestampStart);
            query = query.WhereIf(filter.TimestampEnd.HasValue, x => x.Timestamp <= filter.TimestampEnd);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(filter.ServiceName), x => x.ServiceName == filter.ServiceName);
            query = query.WhereIf(filter.DurationMin.HasValue, x => x.DurationMs >= filter.DurationMin);
            query = query.WhereIf(filter.DurationMax.HasValue, x => x.DurationMs <= filter.DurationMax);
            query = query.WhereIf(filter.MinStatusCode.HasValue, x => x.StatusCode >= filter.MinStatusCode);
            query = query.WhereIf(filter.MaxStatusCode.HasValue, x => x.StatusCode <= filter.MaxStatusCode);

            if (filter.Statuses != null && filter.Statuses.Any())
            {
                query = query.Where(x => filter.Statuses.Contains(x.Status));
            }

            if (filter.HttpMethods != null && filter.HttpMethods.Any())
            {
                query = query.Where(x => filter.HttpMethods.Contains(x.HttpMethod));
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            string searchLower = request.SearchQuery.ToLowerInvariant();
            var searchColumns = request.AllowSearchColumns.Any() ? request.AllowSearchColumns : new[]
            {
                nameof(AuditLogPipelineEntity.ActionName),
                nameof(AuditLogPipelineEntity.ServiceName),
                nameof(AuditLogPipelineEntity.RequestPath),
                nameof(AuditLogPipelineEntity.CorrelationId),
                nameof(AuditLogPipelineEntity.TraceId)
            };

            query = query.SearchDynamic(searchLower, searchColumns, CSearchOperatorType.Contains, true, CSearchCombineModeType.Or);
        }

        return query;
    }
}
