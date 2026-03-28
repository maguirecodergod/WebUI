using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionGrants;

public interface IPermissionGrantRepository : IRepository<PermissionGrantEntity, Guid>
{
    Task<PermissionGrantEntity?> FindAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default);

    Task<List<PermissionGrantEntity>> GetListAsync(
        string providerName, string providerKey, CancellationToken ct = default);

    Task DeleteAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default);

    Task<List<string>> GetPermissionNamesForProviderAsync(
        string providerName, string providerKey, CancellationToken ct = default);
}
