using LHA.Ddd.Domain;
using LHA.EntityFrameworkCore;
using LHA.PermissionManagement.Domain;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionGrants;
using LHA.PermissionManagement.Domain.Shared;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using Microsoft.EntityFrameworkCore;

namespace LHA.PermissionManagement.EntityFrameworkCore;

// ─── PermissionDefinition ────────────────────────────────────────

public sealed class EfCorePermissionDefinitionRepository
    : EfCoreRepository<PermissionManagementDbContext, PermissionDefinitionEntity, Guid>,
      IPermissionDefinitionRepository
{
    private static readonly string[] SearchColumns = ["Name", "DisplayName", "ServiceName"];

    public EfCorePermissionDefinitionRepository(
        IDbContextProvider<PermissionManagementDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<PermissionDefinitionEntity?> FindByNameAsync(string name, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Name == name, ct);
    }

    public async Task<List<PermissionDefinitionEntity>> GetListByNamesAsync(
        IEnumerable<string> names, CancellationToken ct = default)
    {
        var nameList = names.ToList();
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => nameList.Contains(x.Name)).ToListAsync(ct);
    }

    public async Task<List<PermissionDefinitionEntity>> GetListByServiceAsync(
        string serviceName, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => x.ServiceName == serviceName).ToListAsync(ct);
    }

    public async Task<List<PermissionDefinitionEntity>> GetListByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var dbSet = await GetDbSetAsync();
        return await dbSet.Where(x => idList.Contains(x.Id)).ToListAsync(ct);
    }

    public async Task<List<PermissionDefinitionEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(!string.IsNullOrEmpty(serviceName), x => x.ServiceName == serviceName)
            .WhereIf(!string.IsNullOrEmpty(groupName), x => x.GroupName == groupName)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(ct);
    }

    public async Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null, string? groupName = null,
        CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(!string.IsNullOrEmpty(serviceName), x => x.ServiceName == serviceName)
            .WhereIf(!string.IsNullOrEmpty(groupName), x => x.GroupName == groupName)
            .LongCountAsync(ct);
    }

    public async Task<List<PermissionDefinitionEntity>> GetListByMultiTenancySideAsync(
        MultiTenancySides side, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.MultiTenancySide == MultiTenancySides.Both || x.MultiTenancySide == side)
            .ToListAsync(ct);
    }
}

// ─── PermissionGroup ─────────────────────────────────────────────

public sealed class EfCorePermissionGroupRepository
    : EfCoreRepository<PermissionManagementDbContext, PermissionGroupEntity, Guid>,
      IPermissionGroupRepository
{
    private static readonly string[] SearchColumns = ["Name", "DisplayName"];

    public EfCorePermissionGroupRepository(
        IDbContextProvider<PermissionManagementDbContext> dbContextProvider) : base(dbContextProvider) { }

    public override async Task<PermissionGroupEntity?> FindAsync(Guid id, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Include(g => g.Items).FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    public async Task<PermissionGroupEntity?> FindByNameAsync(string name, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Include(g => g.Items).FirstOrDefaultAsync(g => g.Name == name, ct);
    }

    public async Task<List<PermissionGroupEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null, string? serviceName = null,
        CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(g => g.Items)
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(!string.IsNullOrEmpty(serviceName), g => g.ServiceName == serviceName)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(ct);
    }

    public async Task<long> GetCountAsync(
        string? filter = null, string? serviceName = null,
        CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .WhereIf(!string.IsNullOrEmpty(serviceName), g => g.ServiceName == serviceName)
            .LongCountAsync(ct);
    }
}

// ─── PermissionTemplate ──────────────────────────────────────────

public sealed class EfCorePermissionTemplateRepository
    : EfCoreRepository<PermissionManagementDbContext, PermissionTemplateEntity, Guid>,
      IPermissionTemplateRepository
{
    private static readonly string[] SearchColumns = ["Name", "DisplayName"];

    public EfCorePermissionTemplateRepository(
        IDbContextProvider<PermissionManagementDbContext> dbContextProvider) : base(dbContextProvider) { }

    public override async Task<PermissionTemplateEntity?> FindAsync(Guid id, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Include(t => t.Items).FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<PermissionTemplateEntity?> FindByNameAsync(string name, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.Include(t => t.Items).FirstOrDefaultAsync(t => t.Name == name, ct);
    }

    public async Task<List<PermissionTemplateEntity>> GetListAsync(
        PagingParam paging, SorterParam? sorter = null,
        string? filter = null,
        CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(t => t.Items)
            .SearchDynamic(filter, SearchColumns)
            .SortByDynamic(sorter, defaultProperty: "Name")
            .PageBy(paging)
            .ToListAsync(ct);
    }

    public async Task<long> GetCountAsync(string? filter = null, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsQueryable()
            .SearchDynamic(filter, SearchColumns)
            .LongCountAsync(ct);
    }
}

// ─── PermissionGrant ─────────────────────────────────────────────

public sealed class EfCorePermissionGrantRepository
    : EfCoreRepository<PermissionManagementDbContext, PermissionGrantEntity, Guid>,
      IPermissionGrantRepository
{
    public EfCorePermissionGrantRepository(
        IDbContextProvider<PermissionManagementDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<PermissionGrantEntity?> FindAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(
            x => x.Name == name && x.ProviderName == providerName && x.ProviderKey == providerKey, ct);
    }

    public async Task<List<PermissionGrantEntity>> GetListAsync(
        string providerName, string providerKey, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ProviderName == providerName && x.ProviderKey == providerKey)
            .ToListAsync(ct);
    }

    public async Task DeleteAsync(
        string name, string providerName, string providerKey, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        var entity = await dbSet.FirstOrDefaultAsync(
            x => x.Name == name && x.ProviderName == providerName && x.ProviderKey == providerKey, ct);
        if (entity is not null)
            dbSet.Remove(entity);
    }

    public async Task<List<string>> GetPermissionNamesForProviderAsync(
        string providerName, string providerKey, CancellationToken ct = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.ProviderName == providerName && x.ProviderKey == providerKey)
            .Select(x => x.Name)
            .ToListAsync(ct);
    }
}
