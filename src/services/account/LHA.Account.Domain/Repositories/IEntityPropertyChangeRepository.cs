using LHA.AuditLog.Domain;
using LHA.Ddd.Domain;

namespace LHA.Account.Domain.Repositories;

/// <summary>
/// Repository for Entity Property Changes.
/// </summary>
public interface IEntityPropertyChangeRepository : IRepository<EntityPropertyChangeEntity, Guid>
{
    Task<List<EntityPropertyChangeEntity>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        Guid? entityChangeId = null,
        string? propertyName = null,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? entityChangeId = null,
        string? propertyName = null,
        CancellationToken cancellationToken = default);
}
