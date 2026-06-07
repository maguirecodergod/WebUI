using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Shared.Domain.AuditLogActions;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Repository contract for Audit Log Actions.
/// </summary>
public interface IAuditLogActionRepository : IRepository<AuditLogActionEntity, Guid>
{
    Task<PagedResultDto<AuditLogActionEntity>> GetWithPaginationAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default);

    Task<List<AuditLogActionEntity>> GetListAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        AuditLogActionPagedRequest request,
        CancellationToken cancellationToken = default);

    Task<List<AuditLogActionEntity>> GetByAuditLogIdAsync(
        Guid auditLogId,
        CancellationToken cancellationToken = default);
}
