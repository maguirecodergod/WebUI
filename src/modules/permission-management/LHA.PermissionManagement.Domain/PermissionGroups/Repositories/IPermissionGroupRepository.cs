using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionGroups;

public interface IPermissionGroupRepository : IRepository<PermissionGroupEntity, Guid>
{
    Task<PermissionGroupEntity?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionGroupEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, string? serviceName = null,
        CancellationToken ct = default);
    Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null,
        CancellationToken ct = default);
}
