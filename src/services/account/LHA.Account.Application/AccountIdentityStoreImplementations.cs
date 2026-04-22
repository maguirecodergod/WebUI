using LHA.Identity.Domain;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.Shared;
using LHA.Shared.Domain.TenantManagement;

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

        var style = (CMultiTenancyDatabaseStyle)databaseStyle;
        var tenant = await tenantManager.CreateAsync(name, style, ct);
        
        // 1. First, we must persist the tenant to get its ID properly saved
        await tenantRepo.InsertAsync(tenant, ct);

        // 2. Resolve orchestrator and provision infrastructure (e.g., generate conn string, run migrations)
        var provisioner = scope.ServiceProvider.GetRequiredService<LHA.MultiTenancy.Provisioning.ITenantProvisioningOrchestrator>();
        var newConnString = await provisioner.ProvisionAsync(tenant.Id, tenant.NormalizedName, (int)style, ct);

        // 3. Update the tenant again if the provisioner modified the connection strings
        if (!string.IsNullOrWhiteSpace(newConnString))
        {
            tenant.AddOrUpdateConnectionString(LHA.Shared.Domain.TenantManagement.TenantConsts.DefaultConnectionStringName, newConnString);
            await tenantRepo.UpdateAsync(tenant, ct);
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
