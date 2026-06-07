using LHA.UnitOfWork;
using LHA.AuditLog.Domain;
using LHA.Ddd.Application;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.AuditLogs;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore.PostgreSQL;

public class EfCoreAuditLogPostgreSqlRepository
    : EfCoreRepository<AuditLogDbContext, AuditLogEntity, Guid>, IAuditLogRepository
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    public EfCoreAuditLogPostgreSqlRepository(IDbContextProvider<AuditLogDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager)
        : base(dbContextProvider)
    {
        _unitOfWorkManager = unitOfWorkManager;
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
            throw new LHA.Ddd.Domain.EntityNotFoundException(typeof(AuditLogEntity), id);
        }
        return entity;
    }

    public virtual async Task<PagedResultDto<AuditLogEntity>> GetWithPaginationAsync(
        AuditLogPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        ApplyMultiTenantFilter(dbContext, request.DisableTenantFilter);

        var getListQuery = dbContext.Set<AuditLogEntity>().AsNoTracking();
        getListQuery = ApplyFilter(getListQuery, request);

        var countQuery = dbContext.Set<AuditLogEntity>().AsNoTracking();
        countQuery = ApplyFilter(countQuery, request);

        getListQuery = getListQuery.Include(x => x.Actions);
        countQuery = countQuery.Include(x => x.Actions);

        if (request.IncludeDetails)
        {
            getListQuery = getListQuery
                .Include(x => x.EntityChanges)
                .ThenInclude(x => x.PropertyChanges)
                .AsSplitQuery();

            countQuery = countQuery
                .Include(x => x.EntityChanges)
                .ThenInclude(x => x.PropertyChanges)
                .AsSplitQuery();
        }

        var totalCount = await countQuery.LongCountAsync(cancellationToken);

        var list = await getListQuery
            .SortByDynamic(request.Sorter, nameof(AuditLogEntity.ExecutionTime), defaultAscending: false)
            .PageBy(request)
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AuditLogEntity>(
            totalCount,
            list,
            request.PageNumber,
            request.PageSize);
    }

    public virtual async Task<List<AuditLogEntity>> GetListAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default)
    {
        if (input.DisableTenantFilter)
        {
            var dbContext = await GetDbContextAsync();
            ApplyMultiTenantFilter(dbContext, disableFilter: true);
        }

        var query = await GetQueryableAsync(input.IncludeDetails, isReadOnly: true);

        query = ApplyFilter(query, input);

        return await query
            .SortByDynamic(input.Sorter, nameof(AuditLogEntity.ExecutionTime), false)
            .PageBy(input)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default)
    {
        if (input.DisableTenantFilter)
        {
            var dbContext = await GetDbContextAsync();
            ApplyMultiTenantFilter(dbContext, disableFilter: true);
        }

        IQueryable<AuditLogEntity> query = await GetDbSetAsync();

        query = query.AsNoTracking();

        query = ApplyFilter(query, input);

        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.Set<AuditLogEntity>()
            .Where(x => x.ExecutionTime < cutoffTime)
            .ExecuteDeleteAsync(cancellationToken);
    }

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

    protected virtual async Task<IQueryable<AuditLogEntity>> GetQueryableAsync(bool includeDetails, bool isReadOnly = false)
    {
        IQueryable<AuditLogEntity> query = await GetDbSetAsync();

        if (isReadOnly)
        {
            query = query.AsNoTracking();
        }
        
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

    private static void ApplyMultiTenantFilter(DbContext dbContext, bool disableFilter)
    {
        if (!disableFilter)
        {
            return;
        }

        var property = dbContext.GetType().GetProperty(nameof(LhaDbContext<AuditLogDbContext>.IsMultiTenantFilterEnabled));
        property?.SetValue(dbContext, false);
    }
}
