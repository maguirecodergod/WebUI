using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;
using LHA.Auditing;

namespace LHA.Account.Domain.Repositories;

/// <summary>
/// Repository for Entity Changes.
/// </summary>
public interface IEntityChangeRepository : IRepository<EntityChangeEntity, Guid>
{
    Task<List<EntityChangeEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        Guid? auditLogId = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CEntityChangeType? changeType = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? auditLogId = null,
        string? entityTypeFullName = null,
        string? entityId = null,
        CEntityChangeType? changeType = null,
        CancellationToken cancellationToken = default);
}
