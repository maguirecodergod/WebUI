using LHA.Account.Domain.Repositories;
using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LHA.Account.EntityFrameworkCore.Repositories;

public class EfCoreAuditLogActionRepository
    : EfCoreRepository<AccountDbContext, AuditLogActionEntity, Guid>, IAuditLogActionRepository
{
    public EfCoreAuditLogActionRepository(IDbContextProvider<AccountDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<List<AuditLogActionEntity>> GetListAsync(
        Guid? auditLogId = null,
        string? serviceName = null,
        string? methodName = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogActionEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, auditLogId, serviceName, methodName, minExecutionDuration, maxExecutionDuration);

        return await query
            .SortByDynamic(sorting, nameof(AuditLogActionEntity.ExecutionTime), false)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(
        Guid? auditLogId = null,
        string? serviceName = null,
        string? methodName = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AuditLogActionEntity> query = await GetDbSetAsync();

        query = ApplyFilter(query, auditLogId, serviceName, methodName, minExecutionDuration, maxExecutionDuration);

        return await query.LongCountAsync(cancellationToken);
    }

    protected virtual IQueryable<AuditLogActionEntity> ApplyFilter(
        IQueryable<AuditLogActionEntity> query,
        Guid? auditLogId = null,
        string? serviceName = null,
        string? methodName = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null)
    {
        return query
            .WhereIf(auditLogId.HasValue, x => x.AuditLogId == auditLogId)
            .WhereIf(!string.IsNullOrWhiteSpace(serviceName), x => x.ServiceName.Contains(serviceName!))
            .WhereIf(!string.IsNullOrWhiteSpace(methodName), x => x.MethodName.Contains(methodName!))
            .WhereIf(minExecutionDuration.HasValue, x => x.ExecutionDuration >= minExecutionDuration)
            .WhereIf(maxExecutionDuration.HasValue, x => x.ExecutionDuration <= maxExecutionDuration);
    }
}
