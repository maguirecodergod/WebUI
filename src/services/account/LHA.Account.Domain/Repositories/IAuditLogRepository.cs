using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;

namespace LHA.Account.Domain.Repositories;

/// <summary>
/// Specialized repository for Audit Logs within the Account Service.
/// Extends the base repository with advanced filtering and eager loading for AuditLogEntity.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLogEntity, Guid>
{
    Task<AuditLogEntity?> FindAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<AuditLogEntity> GetAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a filtered, paged list of Audit Logs with optional eager loading of children.
    /// </summary>
    Task<List<AuditLogEntity>> GetListAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count matching the filters.
    /// </summary>
    Task<long> GetCountAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes audit logs older than the specified date.
    /// Returns the number of deleted records.
    /// </summary>
    Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default);
}
