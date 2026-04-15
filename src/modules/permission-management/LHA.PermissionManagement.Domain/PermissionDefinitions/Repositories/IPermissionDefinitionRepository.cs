using LHA.Ddd.Domain;
using LHA.PermissionManagement.Domain.Shared;

namespace LHA.PermissionManagement.Domain.PermissionDefinitions;

public interface IPermissionDefinitionRepository : IRepository<PermissionDefinitionEntity, Guid>
{
    Task<PermissionDefinitionEntity?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByServiceAsync(string serviceName, CancellationToken ct = default);
    Task<List<PermissionDefinitionEntity>> GetListByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

    /// <summary>
    /// Gets permissions available for the given multi-tenancy side.
    /// Returns permissions where <see cref="PermissionDefinitionEntity.MultiTenancySide"/>
    /// is <see cref="MultiTenancySides.Both"/> or matches the specified <paramref name="side"/>.
    /// </summary>
    Task<List<PermissionDefinitionEntity>> GetListByMultiTenancySideAsync(
        MultiTenancySides side, CancellationToken ct = default);

    Task<List<PermissionDefinitionEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default);
    Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default);
}