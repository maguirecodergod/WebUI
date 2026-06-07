using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Shared.Domain.AuditLogs;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Repository contract for the Audit Log module.
/// Extends the base repository with audit-specific queries.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLogEntity, Guid>
{
    /// <summary>
    /// Gets a paginated and sorted list of audit logs based on the specified filter criteria.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedResultDto<AuditLogEntity>> GetWithPaginationAsync(
        AuditLogPagedRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds an audit log by its ID, optionally including related details.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuditLogEntity?> FindAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an audit log by its ID, optionally including related details. Throws if not found.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AuditLogEntity> GetAsync(Guid id, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated and sorted list of audit logs based on the specified filter criteria.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<AuditLogEntity>> GetListAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of audit logs matching the specified filter criteria, used for pagination.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetCountAsync(
        AuditLogPagedRequest input,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes audit logs older than the specified cutoff time. Returns the number of logs deleted.
    /// </summary>
    /// <param name="cutoffTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default);
}
