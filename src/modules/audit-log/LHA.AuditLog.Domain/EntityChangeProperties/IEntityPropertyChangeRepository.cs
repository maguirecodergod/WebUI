using LHA.Ddd.Application;
using LHA.Ddd.Domain;
using LHA.Shared.Domain.EntityPropertyChanges;

namespace LHA.AuditLog.Domain.EntityChangeProperties;

/// <summary>
/// Repository contract for Entity Property Changes.
/// </summary>
public interface IEntityPropertyChangeRepository : IRepository<EntityPropertyChangeEntity, Guid>
{
    Task<PagedResultDto<EntityPropertyChangeEntity>> GetWithPaginationAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default);

    Task<List<EntityPropertyChangeEntity>> GetListAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        EntityPropertyChangePagedRequest request,
        CancellationToken cancellationToken = default);
}
