using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionDefinitions;

public interface IPermissionDefinitionRepository : IRepository<PermissionDefinitionEntity, Guid>
{
    Task<PermissionDefinitionEntity?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByServiceAsync(string serviceName, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default);
    Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default);
}