using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain;

// ─── PermissionDefinition ────────────────────────────────────────

public interface IPermissionDefinitionRepository : IRepository<PermissionDefinition, Guid>
{
    Task<PermissionDefinition?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionDefinition>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken ct = default);
    Task<List<PermissionDefinition>> GetListByServiceAsync(string serviceName, CancellationToken ct = default);
    Task<List<PermissionDefinition>> GetListByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<List<PermissionDefinition>> GetListAsync(
        string? filter = null, string? serviceName = null, string? groupName = null,
        string? sorting = null, int skipCount = 0, int maxResultCount = int.MaxValue,
        CancellationToken ct = default);
    Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default);
}

// ─── PermissionGroup ─────────────────────────────────────────────

public interface IPermissionGroupRepository : IRepository<PermissionGroup, Guid>
{
    Task<PermissionGroup?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionGroup>> GetListAsync(
        string? filter = null, string? serviceName = null,
        string? sorting = null, int skipCount = 0, int maxResultCount = int.MaxValue,
        CancellationToken ct = default);
    Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null,
        CancellationToken ct = default);
}

// ─── PermissionTemplate ──────────────────────────────────────────

public interface IPermissionTemplateRepository : IRepository<PermissionTemplate, Guid>
{
    Task<PermissionTemplate?> FindByNameAsync(string name, CancellationToken ct = default);
    Task<List<PermissionTemplate>> GetListAsync(
        string? filter = null, string? sorting = null,
        int skipCount = 0, int maxResultCount = int.MaxValue,
        CancellationToken ct = default);
    Task<long> GetCountAsync(string? filter = null, CancellationToken ct = default);
}

// ─── PermissionGrant ─────────────────────────────────────────────

public interface IPermissionGrantRepository : IRepository<PermissionGrant, Guid>
{
    Task<PermissionGrant?> FindAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default);

    Task<List<PermissionGrant>> GetListAsync(
        string providerName, string providerKey, CancellationToken ct = default);

    Task DeleteAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default);

    Task<List<string>> GetPermissionNamesForProviderAsync(
        string providerName, string providerKey, CancellationToken ct = default);
}
