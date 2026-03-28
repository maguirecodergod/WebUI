using LHA.Ddd.Domain;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Repository contract for the Audit Log module.
/// Extends the base repository with audit-specific queries.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLogEntity, Guid>
{
    /// <summary>
    /// Returns a paged/filtered list of audit logs with eager-loaded children.
    /// </summary>
    Task<List<AuditLogEntity>> GetListAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count matching the given filters.
    /// </summary>
    Task<long> GetCountAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns entity changes filtered by entity type and/or entity ID.
    /// </summary>
    Task<List<EntityChangeEntity>> GetEntityChangesAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of entity changes matching the given filters.
    /// </summary>
    Task<long> GetEntityChangeCountAsync(
        string? entityTypeFullName = null,
        string? entityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes audit logs older than the specified date.
    /// Returns the number of deleted records.
    /// </summary>
    Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default);
}
