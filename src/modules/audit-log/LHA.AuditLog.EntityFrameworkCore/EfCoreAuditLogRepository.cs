using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IAuditLogRepository"/>.
/// Extends the generic <see cref="EfCoreRepository{TDbContext,TEntity,TKey}"/>
/// with audit-log-specific queries including eager-loaded children.
/// </summary>
public sealed class EfCoreAuditLogRepository
    : EfCoreRepository<AuditLogDbContext, AuditLogEntity, Guid>, IAuditLogRepository
{
    public EfCoreAuditLogRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public override async Task<AuditLogEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges)
                .ThenInclude(c => c.PropertyChanges)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        string? httpMethod = null,
        string? url = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        Guid? userId = null,
        string? userName = null,
        string? applicationName = null,
        string? correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilters(dbSet, startTime, endTime, httpMethod, url,
            minStatusCode, maxStatusCode, userId, userName, applicationName,
            correlationId, maxExecutionDuration, minExecutionDuration, hasException);

        return await query
            .SortByDynamic(sorter, defaultProperty: "ExecutionTime", defaultAscending: false)
            .PageBy(paging)
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges)
                .ThenInclude(c => c.PropertyChanges)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetCountAsync(
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        string? httpMethod = null,
        string? url = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        Guid? userId = null,
        string? userName = null,
        string? applicationName = null,
        string? correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilters(dbSet, startTime, endTime, httpMethod, url,
            minStatusCode, maxStatusCode, userId, userName, applicationName,
            correlationId, maxExecutionDuration, minExecutionDuration, hasException);

        return await query.LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<EntityChangeEntity>> GetEntityChangesAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Set<EntityChangeEntity>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(entityTypeFullName),
                c => c.EntityTypeFullName == entityTypeFullName)
            .WhereIf(!string.IsNullOrWhiteSpace(entityId),
                c => c.EntityId == entityId);

        return await query
            .SortByDynamic(sorter, defaultProperty: "ChangeTime", defaultAscending: false)
            .PageBy(paging)
            .Include(c => c.PropertyChanges)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<long> GetEntityChangeCountAsync(
        string? entityTypeFullName = null,
        string? entityId = null,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.Set<EntityChangeEntity>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(entityTypeFullName),
                c => c.EntityTypeFullName == entityTypeFullName)
            .WhereIf(!string.IsNullOrWhiteSpace(entityId),
                c => c.EntityId == entityId)
            .LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        // EF Core 7+ ExecuteDeleteAsync with cascading deletes
        return await dbContext.Set<AuditLogEntity>()
            .Where(x => x.ExecutionTime < cutoffTime)
            .ExecuteDeleteAsync(cancellationToken);
    }

    // ─── Private helpers ─────────────────────────────────────────────

    private static IQueryable<AuditLogEntity> ApplyFilters(
        IQueryable<AuditLogEntity> query,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
        string? httpMethod,
        string? url,
        int? minStatusCode,
        int? maxStatusCode,
        Guid? userId,
        string? userName,
        string? applicationName,
        string? correlationId,
        int? maxExecutionDuration,
        int? minExecutionDuration,
        bool? hasException)
    {
        return query
            .AsNoTracking()
            .WhereIf(startTime.HasValue, x => x.ExecutionTime >= startTime!.Value)
            .WhereIf(endTime.HasValue, x => x.ExecutionTime <= endTime!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(httpMethod), x => x.HttpMethod == httpMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(url), x => x.Url != null && x.Url.Contains(url!))
            .WhereIf(minStatusCode.HasValue, x => x.HttpStatusCode >= minStatusCode!.Value)
            .WhereIf(maxStatusCode.HasValue, x => x.HttpStatusCode <= maxStatusCode!.Value)
            .WhereIf(userId.HasValue, x => x.UserId == userId!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(userName), x => x.UserName != null && x.UserName.Contains(userName!))
            .WhereIf(!string.IsNullOrWhiteSpace(applicationName), x => x.ApplicationName == applicationName)
            .WhereIf(!string.IsNullOrWhiteSpace(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(maxExecutionDuration.HasValue, x => x.ExecutionDuration <= maxExecutionDuration!.Value)
            .WhereIf(minExecutionDuration.HasValue, x => x.ExecutionDuration >= minExecutionDuration!.Value)
            .WhereIf(hasException == true, x => x.Exceptions != null && x.Exceptions != "")
            .WhereIf(hasException == false, x => x.Exceptions == null || x.Exceptions == "");
    }
}
