using LHA.Account.Application.Contracts.Permissions;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using LHA.PermissionManagement.Domain.Shared;
using LHA.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Seeds permission definitions, permission groups, the TenantAdmin permission template,
/// and grants all applicable permissions to the <c>SystemSuperAdmin</c> and <c>TenantAdmin</c> roles.
/// </summary>
/// <remarks>
/// This seeder must run <b>after</b> <see cref="IdentityRoleSeeder"/> because it reads
/// <see cref="SeedingContext.SystemSuperAdminRoleId"/> and <see cref="SeedingContext.TenantAdminRoleId"/>.
/// </remarks>
internal sealed class PermissionSeeder : IDataSeeder
{
    private readonly ILogger _logger;

    public PermissionSeeder(ILogger logger) => _logger = logger;

    public async Task SeedAsync(IServiceProvider serviceProvider, SeedingContext context)
    {
        var uowManager    = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var defRepo       = serviceProvider.GetRequiredService<IPermissionDefinitionRepository>();
        var groupRepo     = serviceProvider.GetRequiredService<IPermissionGroupRepository>();
        var templateRepo  = serviceProvider.GetRequiredService<IPermissionTemplateRepository>();
        var permGrantRepo = serviceProvider.GetRequiredService<LHA.Identity.Domain.IPermissionGrantRepository>();

        using var uow = uowManager.Begin(isTransactional: true);

        var defEntities   = await SeedPermissionDefinitionsAsync(defRepo);
        var groupEntities = await SeedPermissionGroupsAsync(groupRepo, defEntities);

        await SeedTenantAdminTemplateAsync(templateRepo, groupEntities);
        await GrantPermissionsToRoleAsync(permGrantRepo, defEntities, context.SystemSuperAdminRoleId, filterTenantSide: false);
        await GrantPermissionsToRoleAsync(permGrantRepo, defEntities, context.TenantAdminRoleId,      filterTenantSide: true);

        _logger.LogInformation("All permissions granted to 'SystemSuperAdmin' role.");
        _logger.LogInformation("Tenant-specific permissions granted to 'TenantAdmin' role.");

        await uow.CompleteAsync();
    }

    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Upserts every permission definition declared in <see cref="AccountPermissions"/>.
    /// Only the <see cref="MultiTenancySides"/> value is updated if a definition already exists.
    /// </summary>
    private async Task<List<PermissionDefinitionEntity>> SeedPermissionDefinitionsAsync(
        IPermissionDefinitionRepository defRepo)
    {
        var allPermissions = new (string Name, string DisplayName, string Service, string GroupName, MultiTenancySides Side)[]
        {
            // Identity — Users
            (AccountPermissions.UserManagement.Read,         AccountPermissions.UserManagement.L.Read,         "Identity", AccountPermissions.UserManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Create,       AccountPermissions.UserManagement.L.Create,       "Identity", AccountPermissions.UserManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Update,       AccountPermissions.UserManagement.L.Update,       "Identity", AccountPermissions.UserManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Delete,       AccountPermissions.UserManagement.L.Delete,       "Identity", AccountPermissions.UserManagement.GroupName,        MultiTenancySides.Both),
            // Identity — Roles
            (AccountPermissions.RoleManagement.Read,         AccountPermissions.RoleManagement.L.Read,         "Identity", AccountPermissions.RoleManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Create,       AccountPermissions.RoleManagement.L.Create,       "Identity", AccountPermissions.RoleManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Update,       AccountPermissions.RoleManagement.L.Update,       "Identity", AccountPermissions.RoleManagement.GroupName,        MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Delete,       AccountPermissions.RoleManagement.L.Delete,       "Identity", AccountPermissions.RoleManagement.GroupName,        MultiTenancySides.Both),
            // Identity — Claim Types
            (AccountPermissions.ClaimTypeManagement.Read,   AccountPermissions.ClaimTypeManagement.L.Read,   "Identity", AccountPermissions.ClaimTypeManagement.GroupName,   MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Create, AccountPermissions.ClaimTypeManagement.L.Create, "Identity", AccountPermissions.ClaimTypeManagement.GroupName,   MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Update, AccountPermissions.ClaimTypeManagement.L.Update, "Identity", AccountPermissions.ClaimTypeManagement.GroupName,   MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Delete, AccountPermissions.ClaimTypeManagement.L.Delete, "Identity", AccountPermissions.ClaimTypeManagement.GroupName,   MultiTenancySides.Both),
            // Identity — Security Logs
            (AccountPermissions.SecurityLogManagement.Read, AccountPermissions.SecurityLogManagement.L.Read, "Identity", AccountPermissions.SecurityLogManagement.GroupName, MultiTenancySides.Both),
            // Tenant Management (Host Only)
            (AccountPermissions.TenantManagement.Read,   AccountPermissions.TenantManagement.L.Read,   "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Create, AccountPermissions.TenantManagement.L.Create, "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Update, AccountPermissions.TenantManagement.L.Update, "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Delete, AccountPermissions.TenantManagement.L.Delete, "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            // Audit Logs
            (AccountPermissions.AuditLogManagement.Read,     AccountPermissions.AuditLogManagement.L.Read,     "AuditLog", AccountPermissions.AuditLogManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.AuditLogManagement.HostRead, AccountPermissions.AuditLogManagement.L.HostRead, "AuditLog", AccountPermissions.AuditLogManagement.GroupName, MultiTenancySides.Host),
            // Permission Management
            (AccountPermissions.PermissionMgmt.DefinitionsRead,   AccountPermissions.PermissionMgmt.L.DefinitionsRead,   "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.DefinitionsManage, AccountPermissions.PermissionMgmt.L.DefinitionsManage, "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GroupsRead,        AccountPermissions.PermissionMgmt.L.GroupsRead,        "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GroupsManage,      AccountPermissions.PermissionMgmt.L.GroupsManage,      "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.TemplatesRead,     AccountPermissions.PermissionMgmt.L.TemplatesRead,     "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.TemplatesManage,   AccountPermissions.PermissionMgmt.L.TemplatesManage,   "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GrantsRead,        AccountPermissions.PermissionMgmt.L.GrantsRead,        "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GrantsManage,      AccountPermissions.PermissionMgmt.L.GrantsManage,      "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
        };

        var entities = new List<PermissionDefinitionEntity>();

        foreach (var (name, displayName, service, groupName, side) in allPermissions)
        {
            var def = await defRepo.FindByNameAsync(name);
            if (def is null)
            {
                def = new PermissionDefinitionEntity(
                    Guid.NewGuid(), name, displayName, service, groupName, null,
                    multiTenancySide: side);
                await defRepo.InsertAsync(def);
                _logger.LogInformation("Permission '{Name}' created (Side: {Side}).", name, side);
            }
            else if (def.MultiTenancySide != side)
            {
                def.SetMultiTenancySide(side);
                await defRepo.UpdateAsync(def);
                _logger.LogInformation("Permission '{Name}' updated to Side: {Side}.", name, side);
            }

            entities.Add(def);
        }

        return entities;
    }

    private async Task<List<PermissionGroupEntity>> SeedPermissionGroupsAsync(
        IPermissionGroupRepository groupRepo,
        List<PermissionDefinitionEntity> defEntities)
    {
        var groupDefs = new (string Name, string DisplayName, string Service)[]
        {
            (AccountPermissions.UserManagement.GroupName,        AccountPermissions.UserManagement.L.Group,        "Identity"),
            (AccountPermissions.RoleManagement.GroupName,        AccountPermissions.RoleManagement.L.Group,        "Identity"),
            (AccountPermissions.ClaimTypeManagement.GroupName,   AccountPermissions.ClaimTypeManagement.L.Group,   "Identity"),
            (AccountPermissions.SecurityLogManagement.GroupName, AccountPermissions.SecurityLogManagement.L.Group, "Identity"),
            (AccountPermissions.TenantManagement.GroupName,      AccountPermissions.TenantManagement.L.Group,      "TenantManagement"),
            (AccountPermissions.AuditLogManagement.GroupName,    AccountPermissions.AuditLogManagement.L.Group,    "AuditLog"),
            (AccountPermissions.PermissionMgmt.GroupName,        AccountPermissions.PermissionMgmt.L.Group,        "PermissionManagement"),
        };

        var entities = new List<PermissionGroupEntity>();

        foreach (var (name, displayName, service) in groupDefs)
        {
            var grp = await groupRepo.FindByNameAsync(name);
            if (grp is null)
            {
                grp = new PermissionGroupEntity(Guid.NewGuid(), name, displayName, service, null);
                foreach (var def in defEntities.Where(d => d.GroupName == name))
                    grp.AddPermission(def.Id);

                await groupRepo.InsertAsync(grp);
                _logger.LogInformation("Permission group '{Name}' created.", name);
            }

            entities.Add(grp);
        }

        return entities;
    }

    private async Task SeedTenantAdminTemplateAsync(
        IPermissionTemplateRepository templateRepo,
        List<PermissionGroupEntity> groupEntities)
    {
        var adminTemplate = await templateRepo.FindByNameAsync(AccountPermissions.Templates.TenantAdmin.Name);
        if (adminTemplate is not null)
            return;

        adminTemplate = new PermissionTemplateEntity(
            Guid.NewGuid(),
            AccountPermissions.Templates.TenantAdmin.Name,
            AccountPermissions.Templates.TenantAdmin.L_Name,
            AccountPermissions.Templates.TenantAdmin.L_Description);

        foreach (var grp in groupEntities)
            adminTemplate.AddGroup(grp.Id);

        await templateRepo.InsertAsync(adminTemplate);
        _logger.LogInformation("Permission template 'TenantAdmin' created.");
    }

    /// <param name="permGrantRepo">Repository used to check and insert permission grants.</param>
    /// <param name="defEntities">All seeded permission definitions to iterate over.</param>
    /// <param name="roleId">The role that will receive the permission grants.</param>
    /// <param name="filterTenantSide">
    /// When <c>true</c>, only grants permissions with <see cref="MultiTenancySides.Both"/>
    /// or <see cref="MultiTenancySides.Tenant"/> — used for the TenantAdmin role.
    /// </param>
    private static async Task GrantPermissionsToRoleAsync(
        IPermissionGrantRepository permGrantRepo,
        List<PermissionDefinitionEntity> defEntities,
        Guid roleId,
        bool filterTenantSide)
    {
        var roleKey = roleId.ToString();

        var candidates = filterTenantSide
            ? defEntities.Where(d => d.MultiTenancySide is MultiTenancySides.Both or MultiTenancySides.Tenant)
            : defEntities.AsEnumerable();

        foreach (var def in candidates)
        {
            var existing = await permGrantRepo.FindAsync(def.Name, "R", roleKey);
            if (existing is null)
                await permGrantRepo.InsertAsync(new IdentityPermissionGrant(Guid.CreateVersion7(), def.Name, "R", roleKey));
        }
    }
}
