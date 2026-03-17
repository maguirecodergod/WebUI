using LHA.Ddd.Application;
using LHA.Shared.Contracts.AuditLog;

namespace LHA.AuditLog.Application.Contracts;

/// <summary>
/// Application service contract for the Audit Log module.
/// <para>
/// This is a read-only service — audit logs are created exclusively by the
/// <c>IAuditingStore</c> pipeline. There are no create/update/delete operations
/// exposed via the API.
/// </para>
/// </summary>
public interface IAuditLogAppService
{
    /// <summary>
    /// Returns a filtered, paged list of audit logs.
    /// </summary>
    Task<PagedResultDto<AuditLogDto>> GetListAsync(
        GetAuditLogsInput input,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single audit log by its unique identifier, including all
    /// actions, entity changes, and property changes.
    /// </summary>
    Task<AuditLogDto> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filtered, paged list of entity changes across all audit logs.
    /// Useful for viewing the history of a specific entity.
    /// </summary>
    Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(
        GetEntityChangesInput input,
        CancellationToken cancellationToken = default);
}
