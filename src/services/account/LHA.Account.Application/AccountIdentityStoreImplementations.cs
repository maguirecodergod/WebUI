using LHA.Identity.Domain;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using LHA.PermissionManagement.Domain.Shared;
using LHA.Shared.Domain.TenantManagement;
using LHA.MultiTenancy;

namespace LHA.Account.Application;

/// <summary>
/// Implements Identity module abstractions by bridging with Tenant and Permission modules.
/// </summary>
public sealed class AccountUserTenantLookupService : IUserTenantLookupService, ITenantManagerBridge
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountUserTenantLookupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<Guid> CreateTenantAsync(string name, int databaseStyle, CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var tenantManager = scope.ServiceProvider.GetRequiredService<LHA.TenantManagement.Domain.TenantManager>();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<LHA.TenantManagement.Domain.ITenantRepository>();

        // Fetch permission metadata from Host database before provisioning
        var defRepo = scope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();
        var groupRepo = scope.ServiceProvider.GetRequiredService<IPermissionGroupRepository>();
        var templateRepo = scope.ServiceProvider.GetRequiredService<IPermissionTemplateRepository>();
        var hostDefs = await defRepo.GetListAsync(cancellationToken: ct);
        var hostGroups = await groupRepo.GetAllWithItemsAsync(ct);
        var hostTemplates = await templateRepo.GetAllWithItemsAsync(ct);

        var style = (CMultiTenancyDatabaseStyle)databaseStyle;
        var tenant = await tenantManager.CreateAsync(name, style, ct);
        
        // 1. First, we must persist the tenant to get its ID properly saved
        await tenantRepo.InsertAsync(tenant, ct);

        // 2. Resolve orchestrator and provision infrastructure (e.g., generate conn string, run migrations)
        var provisioner = scope.ServiceProvider.GetRequiredService<LHA.MultiTenancy.Provisioning.ITenantProvisioningOrchestrator>();
        var newConnString = await provisioner.ProvisionAsync(tenant.Id, tenant.NormalizedName, (int)style, ct);

        // Register the new connection string in the current Unit of Work items, so nested UoWs can resolve it immediately
        var uowManager = scope.ServiceProvider.GetRequiredService<LHA.UnitOfWork.IUnitOfWorkManager>();
        var currentUow = uowManager.Current;
        if (currentUow is not null && !string.IsNullOrWhiteSpace(newConnString))
        {
            currentUow.Items["TenantConnectionString_" + tenant.Id] = newConnString;
        }

        // 3. Update the tenant again if the provisioner modified the connection strings
        if (!string.IsNullOrWhiteSpace(newConnString))
        {
            tenant.AddOrUpdateConnectionString(LHA.Shared.Domain.TenantManagement.TenantConsts.DefaultConnectionStringName, newConnString);
            await tenantRepo.UpdateAsync(tenant, ct);
        }

        // 4. Seed permission metadata (definitions, groups, templates) to the newly created dedicated database
        if (style == CMultiTenancyDatabaseStyle.PerTenant)
        {
            var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();

            using (currentTenant.Change(tenant.Id))
            {
                await using (var uow = uowManager.Begin(new LHA.UnitOfWork.UnitOfWorkOptions { IsTransactional = true }, requiresNew: true))
                {
                    var tenantDefRepo = scope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();
                    var tenantGroupRepo = scope.ServiceProvider.GetRequiredService<IPermissionGroupRepository>();
                    var tenantTemplateRepo = scope.ServiceProvider.GetRequiredService<IPermissionTemplateRepository>();

                    // Copy only Tenant-visible Definitions (Both or Tenant side, never Host-only)
                    var defIdMap = new Dictionary<Guid, Guid>();
                    foreach (var hostDef in hostDefs.Where(d => d.MultiTenancySide != LHA.PermissionManagement.Domain.Shared.MultiTenancySides.Host))
                    {
                        var newDefId = Guid.NewGuid();
                        defIdMap[hostDef.Id] = newDefId;
                        await tenantDefRepo.InsertAsync(new PermissionDefinitionEntity(
                            newDefId,
                            hostDef.Name,
                            hostDef.DisplayName,
                            hostDef.ServiceName,
                            hostDef.GroupName,
                            hostDef.Description,
                            hostDef.MultiTenancySide), ct);
                    }

                    // Copy Groups to Tenant DB — skip groups whose permissions are all Host-only
                    var groupIdMap = new Dictionary<Guid, Guid>();
                    foreach (var hostGroup in hostGroups)
                    {
                        var tenantItems = hostGroup.Items
                            .Where(item => defIdMap.ContainsKey(item.PermissionDefinitionId))
                            .ToList();

                        // Skip groups that have no Tenant-visible permissions
                        if (tenantItems.Count == 0) continue;

                        var newGroupId = Guid.NewGuid();
                        groupIdMap[hostGroup.Id] = newGroupId;
                        var tenantGroup = new PermissionGroupEntity(
                            newGroupId,
                            hostGroup.Name,
                            hostGroup.DisplayName,
                            hostGroup.ServiceName,
                            hostGroup.Description);

                        foreach (var hostItem in tenantItems)
                        {
                            tenantGroup.AddPermission(defIdMap[hostItem.PermissionDefinitionId]);
                        }

                        await tenantGroupRepo.InsertAsync(tenantGroup, ct);
                    }

                    // Copy Templates to Tenant DB
                    foreach (var hostTemplate in hostTemplates)
                    {
                        var tenantTemplate = new PermissionTemplateEntity(
                            Guid.NewGuid(),
                            hostTemplate.Name,
                            hostTemplate.DisplayName,
                            hostTemplate.Description);

                        foreach (var hostItem in hostTemplate.Items)
                        {
                            if (groupIdMap.TryGetValue(hostItem.PermissionGroupId, out var newGroupId))
                            {
                                tenantTemplate.AddGroup(newGroupId);
                            }
                        }

                        await tenantTemplateRepo.InsertAsync(tenantTemplate, ct);
                    }

                    await uow.CompleteAsync();
                }
            }
        }

        return tenant.Id;
    }

    public async Task<List<(Guid Id, string Name)>> GetTenantsAsync(List<Guid> tenantIds, CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

        // As batch get is not in base IRepository, we fetch all and filter for now.
        // In a real production system, this should be a custom repo method with "WHERE Id IN (...)".
        var all = await repo.GetListAsync(ct);
        return all.Where(t => tenantIds.Contains(t.Id)).Select(t => (t.Id, t.Name)).ToList();
    }
}

/// <summary>
/// Implements Identity module abstractions by bridging with Permission module.
/// </summary>
public sealed class AccountPermissionStore : IPermissionStore
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountPermissionStore(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<List<string>> GetAllPermissionsAsync(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();

        var all = await repo.GetListAsync(cancellationToken: ct);
        return all.ConvertAll(p => p.Name);
    }

    public async Task<List<string>> GetTenantPermissionsAsync(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();

        var tenantPermissions = await repo.GetListByMultiTenancySideAsync(
            MultiTenancySides.Tenant, ct);
        return tenantPermissions.ConvertAll(p => p.Name);
    }
}
