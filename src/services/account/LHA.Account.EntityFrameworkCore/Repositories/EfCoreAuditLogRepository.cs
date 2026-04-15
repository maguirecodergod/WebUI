using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using LHA.Shared.Domain.Requests;
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
        AuditLogGetListInput input,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync(input.IncludeDetails);

        query = ApplyFilter(query, input);

        return await query
            .SortByDynamic(input.Sorter, nameof(AuditLogEntity.ExecutionTime), false)
            .PageBy(input)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        AuditLogGetListInput input,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogEntity> query = await GetDbSetAsync();

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
        AuditLogGetListInput input)
    {
        var filter = input.Filter;
        if (filter == null)
        {
            return query.SearchDynamic(input.SearchQuery, [nameof(AuditLogEntity.Url), nameof(AuditLogEntity.UserName), nameof(AuditLogEntity.ApplicationName)]);
        }

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
            .SearchDynamic(input.SearchQuery, [nameof(AuditLogEntity.Url), nameof(AuditLogEntity.UserName), nameof(AuditLogEntity.ApplicationName)]);
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
