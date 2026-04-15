using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.Shared.Domain.Requests;

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
        AuditLogGetListInput input,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count matching the filters.
    /// </summary>
    Task<long> GetCountAsync(
        AuditLogGetListInput input,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes audit logs older than the specified date.
    /// Returns the number of deleted records.
    /// </summary>
    Task<int> DeleteOlderThanAsync(
        DateTimeOffset cutoffTime,
        CancellationToken cancellationToken = default);
}
