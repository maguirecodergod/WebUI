using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.Account.EntityFrameworkCore.Repositories;

public class EfCoreAuditLogRepository
    : EfCoreRepository<AccountDbContext, AuditLogEntity, Guid>, LHA.Account.Domain.Repositories.IAuditLogRepository
{
    public EfCoreAuditLogRepository(IDbContextProvider<AccountDbContext> dbContextProvider)
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
            throw new LHA.Ddd.Domain.EntityNotFoundException(typeof(AuditLogEntity), id);
        }
        return entity;
    }

    public virtual async Task<List<AuditLogEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        string? httpMethod = null,
        string? url = null,
        Guid? userId = null,
        string? userName = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        string? applicationName = null,
        string? correlationId = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        bool? hasException = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(includeDetails);

        query = ApplyFilter(query, startTime, endTime, httpMethod, url, userId, userName, 
            minStatusCode, maxStatusCode, applicationName, correlationId, 
            minExecutionDuration, maxExecutionDuration, hasException);

        return await query
            .SortByDynamic(sorter, nameof(AuditLogEntity.ExecutionTime), false)
            .PageBy(paging)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        string? httpMethod = null,
        string? url = null,
        Guid? userId = null,
        string? userName = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        string? applicationName = null,
        string? correlationId = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        bool? hasException = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, startTime, endTime, httpMethod, url, userId, userName, 
            minStatusCode, maxStatusCode, applicationName, correlationId, 
            minExecutionDuration, maxExecutionDuration, hasException);

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
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        string? httpMethod = null,
        string? url = null,
        Guid? userId = null,
        string? userName = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        string? applicationName = null,
        string? correlationId = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        bool? hasException = null)
    {
        return query
            .WhereIf(startTime.HasValue, x => x.ExecutionTime >= startTime)
            .WhereIf(endTime.HasValue, x => x.ExecutionTime <= endTime)
            .WhereIf(!string.IsNullOrWhiteSpace(httpMethod), x => x.HttpMethod == httpMethod)
            .WhereIf(!string.IsNullOrWhiteSpace(url), x => x.Url != null && x.Url.Contains(url!))
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(!string.IsNullOrWhiteSpace(userName), x => x.UserName == userName)
            .WhereIf(minStatusCode.HasValue, x => x.HttpStatusCode >= minStatusCode)
            .WhereIf(maxStatusCode.HasValue, x => x.HttpStatusCode <= maxStatusCode)
            .WhereIf(!string.IsNullOrWhiteSpace(applicationName), x => x.ApplicationName == applicationName)
            .WhereIf(!string.IsNullOrWhiteSpace(correlationId), x => x.CorrelationId == correlationId)
            .WhereIf(minExecutionDuration.HasValue, x => x.ExecutionDuration >= minExecutionDuration)
            .WhereIf(maxExecutionDuration.HasValue, x => x.ExecutionDuration <= maxExecutionDuration)
            .WhereIf(hasException.HasValue && hasException.Value, x => x.Exceptions != null && x.Exceptions != "" && x.Exceptions != "[]")
            .WhereIf(hasException.HasValue && !hasException.Value, x => x.Exceptions == null || x.Exceptions == "" || x.Exceptions == "[]");
    }

    protected virtual async Task<IQueryable<AuditLogEntity>> GetQueryableAsync(bool includeDetails)
    {
        IQueryable<AuditLogEntity> query = await GetDbSetAsync();
        if (includeDetails)
        {
            query = query
                .Include(x => x.Actions)
                .Include(x => x.EntityChanges)
                .ThenInclude(x => x.PropertyChanges)
                .AsSplitQuery();
        }
        return query;
    }
}
