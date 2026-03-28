using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;

namespace LHA.Account.Domain.Repositories;

/// <summary>
/// Repository for Audit Log Actions.
/// </summary>
public interface IAuditLogActionRepository : IRepository<AuditLogActionEntity, Guid>
{
    Task<List<AuditLogActionEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        Guid? auditLogId = null,
        string? serviceName = null,
        string? methodName = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? auditLogId = null,
        string? serviceName = null,
        string? methodName = null,
        int? minExecutionDuration = null,
        int? maxExecutionDuration = null,
        CancellationToken cancellationToken = default);
}
