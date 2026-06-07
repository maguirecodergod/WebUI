using LHA.Ddd.Application;
using LHA.Shared.Contracts.AuditLog;
using LHA.Shared.Domain.AuditLogActions;
using LHA.Shared.Domain.AuditLogs;
using LHA.Shared.Domain.EntityChanges;
using LHA.Shared.Domain.EntityPropertyChanges;

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
    Task<PagedResultDto<AuditLogDto>> GetAuditLogWithPaginationAsync(
        AuditLogPagedRequest pagedRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single audit log by its unique identifier, including all
    /// actions, entity changes, and property changes.
    /// </summary>
    Task<AuditLogDto> GetAuditLogDetailAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an audit log by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAuditLogAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes audit logs older than the specified offset.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> DeleteAuditLogOlderThanAsync(DateTimeOffset offset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filtered, paged list of entity changes across all audit logs.
    /// Useful for viewing the history of a specific entity.
    /// </summary>
    Task<PagedResultDto<EntityChangeDto>> GetEntityChangesWithPaginationAsync(
        EntityChangePagedRequest pagedRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filtered, paged list of audit log actions.
    /// </summary>
    /// <param name="pagedRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedResultDto<AuditLogActionDto>> GetAuditLogActionsWithPaginationAsync(
        AuditLogActionPagedRequest pagedRequest,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Returns a single audit log action by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuditLogActionDto> GetAuditLogActionDetailAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filtered, paged list of entity property changes.
    /// Useful for viewing the history of a specific property.
    /// </summary>
    /// <param name="pagedRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedResultDto<EntityPropertyChangeDto>> GetEntityPropertyChangesWithPaginationAsync(
        EntityPropertyChangePagedRequest pagedRequest,
        CancellationToken cancellationToken = default);
}
