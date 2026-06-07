using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Shared.Domain.EntityChanges;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Repository contract for Entity Changes.
/// </summary>
public interface IEntityChangeRepository : IRepository<EntityChangeEntity, Guid>
{
    /// <summary>
    /// Get entity changes with pagination.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PagedResultDto<EntityChangeEntity>> GetWithPaginationAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entity changes list.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<EntityChangeEntity>> GetListAsync(
        EntityChangePagedRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entity changes count.
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// </summary>
    Task<long> GetCountAsync(EntityChangePagedRequest request,
        CancellationToken cancellationToken = default);
}
