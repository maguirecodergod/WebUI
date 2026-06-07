using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.Ddd.Application;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.AuditLogs;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB;

/// <summary>
/// MongoDB-specific implementation of <see cref="IAuditLogRepository"/>.
/// Handles MongoDB-specific behaviors like lack of ExecuteDeleteAsync support.
/// </summary>
public class EfCoreAuditLogMongoDbRepository
    : EfCoreRepository<AuditLogDbContext, AuditLogEntity, Guid>, IAuditLogRepository
{
    public EfCoreAuditLogMongoDbRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<AuditLogEntity?> FindAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(includeDetails);
        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<AuditLogEntity> GetAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);
        if (entity is null)
        {
            throw new EntityNotFoundException(typeof(AuditLogEntity), id);
        }
        return entity;
    }

    /// <inheritdoc />
    public override async Task<AuditLogEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        // MongoDB doesn't support AsSplitQuery, so we skip it
        return await dbSet
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges)
                .ThenInclude(c => c.PropertyChanges)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AuditLogEntity>> GetWithPaginationAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(dbSet, input);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var queryFinal = query
            .SortByDynamic(input.Sorter, defaultProperty: nameof(AuditLogEntity.ExecutionTime), defaultAscending: false)
            .PageBy(input);

        // MongoDB doesn't support AsSplitQuery
        var items = await queryFinal
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges)
                .ThenInclude(c => c.PropertyChanges)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AuditLogEntity>(totalCount, items, input.PageNumber, input.PageSize);
    }

    /// <inheritdoc />
    public async Task<List<AuditLogEntity>> GetListAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();

        var query = ApplyFilter(dbSet, input);

        var queryFinal = query
            .SortByDynamic(input.Sorter, defaultProperty: nameof(AuditLogEntity.ExecutionTime), defaultAscending: false)
            .PageBy(input);

        // MongoDB doesn't support AsSplitQuery
        return await queryFinal
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges)
                .ThenInclude(c => c.PropertyChanges)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<long> GetCountAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default)
    {
        if (input.DisableTenantFilter)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext is LhaDbContext<AuditLogDbContext> lhaDb)
                lhaDb.IsMultiTenantFilterEnabled = false;
        }

        IQueryable<AuditLogEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, input);

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

        var finalQuery = query
            .SortByDynamic(sorter, defaultProperty: nameof(EntityChangeEntity.ChangeTime), defaultAscending: false)
            .PageBy(paging);

        return await finalQuery
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

        // MongoDB doesn't support ExecuteDeleteAsync, so we fetch and delete
        var logsToDelete = await dbContext.Set<AuditLogEntity>()
            .Where(x => x.ExecutionTime < cutoffTime)
            .ToListAsync(cancellationToken);

        if (logsToDelete.Count == 0) return 0;

        dbContext.Set<AuditLogEntity>().RemoveRange(logsToDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        return logsToDelete.Count;
    }

    // ─── Private helpers ─────────────────────────────────────────────

    protected virtual IQueryable<AuditLogEntity> ApplyFilter(
        IQueryable<AuditLogEntity> query,
        AuditLogPagedRequest input)
    {
        var filter = input.Filter;
        if (filter == null)
        {
            var defaultColumns = new[] { nameof(AuditLogEntity.Url), nameof(AuditLogEntity.UserName), nameof(AuditLogEntity.ApplicationName) };
            var searchColumns = input.AllowSearchColumns?.Length > 0 ? input.AllowSearchColumns : defaultColumns;
            return query.SearchDynamic(input.SearchQuery, searchColumns);
        }

        var filterDefaultColumns = new[] { nameof(AuditLogEntity.Url), nameof(AuditLogEntity.UserName), nameof(AuditLogEntity.ApplicationName) };
        var filterSearchColumns = input.AllowSearchColumns?.Length > 0 ? input.AllowSearchColumns : filterDefaultColumns;
        return query
            .WhereIf(filter.StartTime.HasValue, x => x.ExecutionTime >= filter.StartTime)
            .WhereIf(filter.EndTime.HasValue, x => x.ExecutionTime <= filter.EndTime)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.HttpMethod), x => x.HttpMethod == filter.HttpMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Url), x => x.Url != null && x.Url.Contains(filter.Url!))
            .WhereIf(filter.UserId.HasValue, x => x.UserId == filter.UserId)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.UserName), x => x.UserName == filter.UserName)
            .WhereIf(filter.MinStatusCode.HasValue, x => x.HttpStatusCode >= filter.MinStatusCode)
            .WhereIf(filter.MaxStatusCode.HasValue, x => x.HttpStatusCode <= filter.MaxStatusCode)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.ApplicationName), x => x.ApplicationName == filter.ApplicationName)
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CorrelationId), x => x.CorrelationId == filter.CorrelationId)
            .WhereIf(filter.MinExecutionDuration.HasValue, x => x.ExecutionDuration >= filter.MinExecutionDuration)
            .WhereIf(filter.MaxExecutionDuration.HasValue, x => x.ExecutionDuration <= filter.MaxExecutionDuration)
            .WhereIf(filter.HasException.HasValue && filter.HasException.Value, x => x.Exceptions != null && x.Exceptions != "" && x.Exceptions != "[]")
            .WhereIf(filter.HasException.HasValue && !filter.HasException.Value, x => x.Exceptions == null || x.Exceptions == "" || x.Exceptions == "[]")
            .SearchDynamic(input.SearchQuery, filterSearchColumns);
    }

    protected virtual async Task<IQueryable<AuditLogEntity>> GetQueryableAsync(bool includeDetails)
    {
        IQueryable<AuditLogEntity> query = await GetDbSetAsync();
        
        query = query.Include(x => x.Actions);

        if (includeDetails)
        {
            query = query
                .Include(x => x.EntityChanges)
                .ThenInclude(x => x.PropertyChanges)
                .AsSplitQuery();
        }
        return query;
    }
}
