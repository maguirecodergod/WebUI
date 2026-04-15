using LHA.Account.Application;
using LHA.Account.Application.Contracts.Permissions;
using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.Core.Users;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using LHA.TenantManagement.Domain;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LHA.EventBus;
using LHA.DistributedLocking;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.Shared;
using LHA.Localization;
using LHA.Identity.Domain.Shared.Localization;
using LHA.Account.Domain.Shared.Localization;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHAInMemoryEventBus();
builder.Services.AddLHADistributedLocking();
builder.Services.AddSingleton<IClientInfoProvider, NullClientInfoProvider>();

// ── Localization ─────────────────────────────────────────────────
builder.Services.AddLHALocalization(options =>
{
    options.AddResource<IdentityResource>();
    options.AddResource<AccountResource>();
});

// ── Module services (Application + EF Core) ──────────────────────
builder.Services.AddAccountApplication();
builder.Services.AddAccountEntityFrameworkCore(connectionString);

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Account.Migrator");

// ══════════════════════════════════════════════════════════════════
// 1. Apply pending migrations
// ══════════════════════════════════════════════════════════════════
logger.LogInformation("Applying Account Service migrations...");

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    await dbContext.Database.MigrateAsync();
}

logger.LogInformation("Account Service migrations applied successfully.");

// ══════════════════════════════════════════════════════════════════
// 2. Seed data
// ══════════════════════════════════════════════════════════════════
logger.LogInformation("Seeding default data...");

using (var scope = host.Services.CreateScope())
{
    var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

    // Track the admin role ID for permission grant seeding later
    Guid systemSuperAdminRoleId;
    Guid tenantAdminRoleId;

    // ── 2a. Identity: admin roles + admin user ───────────────────
    var roleManager = scope.ServiceProvider.GetRequiredService<IdentityRoleManager>();
    var roleRepository = scope.ServiceProvider.GetRequiredService<IIdentityRoleRepository>();
    var userManager = scope.ServiceProvider.GetRequiredService<IdentityUserManager>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IIdentityUserRepository>();

    using (var uow = uowManager.Begin(isTransactional: true))
    {
        // ── SystemSuperAdmin Role ──
        var existingSuperAdmin = await roleRepository.FindByNormalizedNameAsync(
            CurrentUserDefaults.SystemSuperAdminRoleName.ToUpperInvariant());

        IdentityRole systemSuperAdminRole;
        if (existingSuperAdmin is null)
        {
            systemSuperAdminRole = await roleManager.CreateAsync(
                CurrentUserDefaults.SystemSuperAdminRoleName,
                isStatic: true,
                isPublic: true);
            await roleRepository.InsertAsync(systemSuperAdminRole);
            logger.LogInformation("SystemSuperAdmin role created.");
        }
        else
        {
            systemSuperAdminRole = existingSuperAdmin;
        }
        systemSuperAdminRoleId = systemSuperAdminRole.Id;

        // ── TenantAdmin Role ──
        var existingTenantAdmin = await roleRepository.FindByNormalizedNameAsync(
            CurrentUserDefaults.TenantAdminRoleName.ToUpperInvariant());

        IdentityRole tenantAdminRole;
        if (existingTenantAdmin is null)
        {
            tenantAdminRole = await roleManager.CreateAsync(
                CurrentUserDefaults.TenantAdminRoleName,
                isStatic: true,
                isPublic: true);
            await roleRepository.InsertAsync(tenantAdminRole);
            logger.LogInformation("TenantAdmin role created.");
        }
        else
        {
            tenantAdminRole = existingTenantAdmin;
        }
        tenantAdminRoleId = tenantAdminRole.Id;

        var existingUser = await userRepository.FindByNormalizedUserNameAsync(
            CurrentUserDefaults.AdminUserName.ToUpperInvariant());

        if (existingUser is null)
        {
            var adminUser = await userManager.CreateAsync(
                CurrentUserDefaults.AdminUserName,
                CurrentUserDefaults.AdminUserEmail,
                CurrentUserDefaults.AdminUserDefaultPassword,
                CurrentUserDefaults.AdminUserId);
            adminUser.AddRole(systemSuperAdminRoleId);
            adminUser.AddRole(tenantAdminRoleId);
            await userRepository.InsertAsync(adminUser);
            logger.LogInformation("Admin user '{UserName}' created (ID: {UserId}).",
                adminUser.UserName, adminUser.Id);
        }
        else
        {
            logger.LogInformation("Admin user already exists, skipping.");
        }

        await uow.CompleteAsync();
    }

    // ── 2b. Tenant Management: default tenant ────────────────────
    var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
    var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

    using (var uow = uowManager.Begin(isTransactional: true))
    {
        var existing = await tenantRepo.FindByNameAsync(
            CurrentUserDefaults.DefaultTenantName.ToUpperInvariant());

        if (existing is null)
        {
            var tenant = await tenantManager.CreateAsync(
                CurrentUserDefaults.DefaultTenantName,
                CurrentUserDefaults.DefaultTenantId);
            await tenantRepo.InsertAsync(tenant);
            logger.LogInformation("Default tenant '{TenantName}' created (ID: {TenantId}).",
                tenant.Name, tenant.Id);
        }
        else
        {
            logger.LogInformation("Default tenant already exists, skipping.");
        }

        await uow.CompleteAsync();
    }

    // ── 2c. Permission Management: definitions, groups, template ─
    var defRepo = scope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();
    var groupRepo = scope.ServiceProvider.GetRequiredService<IPermissionGroupRepository>();
    var templateRepo = scope.ServiceProvider.GetRequiredService<IPermissionTemplateRepository>();
    var permGrantRepo = scope.ServiceProvider
        .GetRequiredService<LHA.Identity.Domain.IPermissionGrantRepository>();

    using (var uow = uowManager.Begin(isTransactional: true))
    {
        // ── All permission definitions ───────────────────────────
        var allPermissions = new (string Name, string DisplayName, string Service, string GroupName, MultiTenancySides Side)[]
        {
            // Identity — Users
            (AccountPermissions.UserManagement.Read,         AccountPermissions.UserManagement.L.Read,         "Identity", AccountPermissions.UserManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Create,       AccountPermissions.UserManagement.L.Create,       "Identity", AccountPermissions.UserManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Update,       AccountPermissions.UserManagement.L.Update,       "Identity", AccountPermissions.UserManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.UserManagement.Delete,       AccountPermissions.UserManagement.L.Delete,       "Identity", AccountPermissions.UserManagement.GroupName, MultiTenancySides.Both),
            // Identity — Roles
            (AccountPermissions.RoleManagement.Read,         AccountPermissions.RoleManagement.L.Read,         "Identity", AccountPermissions.RoleManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Create,       AccountPermissions.RoleManagement.L.Create,       "Identity", AccountPermissions.RoleManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Update,       AccountPermissions.RoleManagement.L.Update,       "Identity", AccountPermissions.RoleManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.RoleManagement.Delete,       AccountPermissions.RoleManagement.L.Delete,       "Identity", AccountPermissions.RoleManagement.GroupName, MultiTenancySides.Both),
            // Identity — Claim Types
            (AccountPermissions.ClaimTypeManagement.Read,   AccountPermissions.ClaimTypeManagement.L.Read,   "Identity", AccountPermissions.ClaimTypeManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Create, AccountPermissions.ClaimTypeManagement.L.Create, "Identity", AccountPermissions.ClaimTypeManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Update, AccountPermissions.ClaimTypeManagement.L.Update, "Identity", AccountPermissions.ClaimTypeManagement.GroupName, MultiTenancySides.Both),
            (AccountPermissions.ClaimTypeManagement.Delete, AccountPermissions.ClaimTypeManagement.L.Delete, "Identity", AccountPermissions.ClaimTypeManagement.GroupName, MultiTenancySides.Both),
            // Identity — Security Logs
            (AccountPermissions.SecurityLogManagement.Read, AccountPermissions.SecurityLogManagement.L.Read, "Identity", AccountPermissions.SecurityLogManagement.GroupName, MultiTenancySides.Both),
            // Tenant Management (Host Only)
            (AccountPermissions.TenantManagement.Read,                AccountPermissions.TenantManagement.L.Read,       "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Create,              AccountPermissions.TenantManagement.L.Create,     "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Update,              AccountPermissions.TenantManagement.L.Update,     "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            (AccountPermissions.TenantManagement.Delete,              AccountPermissions.TenantManagement.L.Delete,     "TenantManagement", AccountPermissions.TenantManagement.GroupName, MultiTenancySides.Host),
            // Audit Logs
            (AccountPermissions.AuditLogManagement.Read,             AccountPermissions.AuditLogManagement.L.Read,    "AuditLog", AccountPermissions.AuditLogManagement.GroupName, MultiTenancySides.Both),
            // Permission Management
            (AccountPermissions.PermissionMgmt.DefinitionsRead,   AccountPermissions.PermissionMgmt.L.DefinitionsRead,   "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.DefinitionsManage, AccountPermissions.PermissionMgmt.L.DefinitionsManage, "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Host),
            (AccountPermissions.PermissionMgmt.GroupsRead,        AccountPermissions.PermissionMgmt.L.GroupsRead,        "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GroupsManage,      AccountPermissions.PermissionMgmt.L.GroupsManage,      "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Host),
            (AccountPermissions.PermissionMgmt.TemplatesRead,     AccountPermissions.PermissionMgmt.L.TemplatesRead,     "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.TemplatesManage,   AccountPermissions.PermissionMgmt.L.TemplatesManage,   "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Host),
            (AccountPermissions.PermissionMgmt.GrantsRead,        AccountPermissions.PermissionMgmt.L.GrantsRead,        "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Both),
            (AccountPermissions.PermissionMgmt.GrantsManage,      AccountPermissions.PermissionMgmt.L.GrantsManage,      "PermissionManagement", AccountPermissions.PermissionMgmt.GroupName, MultiTenancySides.Host),
        };

        var defEntities = new List<PermissionDefinitionEntity>();
        foreach (var (name, displayName, service, groupName, side) in allPermissions)
        {
            var def = await defRepo.FindByNameAsync(name);
            if (def is null)
            {
                def = new PermissionDefinitionEntity(
                    Guid.NewGuid(), name, displayName, service, groupName, null,
                    multiTenancySide: side);
                await defRepo.InsertAsync(def);
                logger.LogInformation("Permission '{Name}' created (Side: {Side}).", name, side);
            }
            else if (def.MultiTenancySide != side)
            {
                def.SetMultiTenancySide(side);
                await defRepo.UpdateAsync(def);
                logger.LogInformation("Permission '{Name}' updated to Side: {Side}.", name, side);
            }
            defEntities.Add(def);
        }

        // ── Permission groups ────────────────────────────────────
        var groupDefs = new (string Name, string DisplayName, string Service)[]
        {
            (AccountPermissions.UserManagement.GroupName,       AccountPermissions.UserManagement.L.Group,        "Identity"),
            (AccountPermissions.RoleManagement.GroupName,       AccountPermissions.RoleManagement.L.Group,        "Identity"),
            (AccountPermissions.ClaimTypeManagement.GroupName,  AccountPermissions.ClaimTypeManagement.L.Group,  "Identity"),
            (AccountPermissions.SecurityLogManagement.GroupName,AccountPermissions.SecurityLogManagement.L.Group, "Identity"),
            (AccountPermissions.TenantManagement.GroupName,     AccountPermissions.TenantManagement.L.Group,      "TenantManagement"),
            (AccountPermissions.AuditLogManagement.GroupName,   AccountPermissions.AuditLogManagement.L.Group,   "AuditLog"),
            (AccountPermissions.PermissionMgmt.GroupName,       AccountPermissions.PermissionMgmt.L.Group,       "PermissionManagement"),
        };

        var groupEntities = new List<PermissionGroupEntity>();
        foreach (var (name, displayName, service) in groupDefs)
        {
            var grp = await groupRepo.FindByNameAsync(name);
            if (grp is null)
            {
                grp = new PermissionGroupEntity(Guid.NewGuid(), name, displayName, service, null);
                foreach (var def in defEntities.Where(d => d.GroupName == name))
                    grp.AddPermission(def.Id);
                await groupRepo.InsertAsync(grp);
                logger.LogInformation("Permission group '{Name}' created.", name);
            }
            groupEntities.Add(grp);
        }

        // ── Permission template: TenantAdmin (full access) ──────
        var adminTemplate = await templateRepo.FindByNameAsync(AccountPermissions.Templates.TenantAdmin.Name);
        if (adminTemplate is null)
        {
            adminTemplate = new PermissionTemplateEntity(
                Guid.NewGuid(), AccountPermissions.Templates.TenantAdmin.Name, 
                AccountPermissions.Templates.TenantAdmin.L_Name,
                AccountPermissions.Templates.TenantAdmin.L_Description);
            foreach (var grp in groupEntities)
                adminTemplate.AddGroup(grp.Id);
            await templateRepo.InsertAsync(adminTemplate);
            logger.LogInformation("Permission template 'TenantAdmin' created.");
        }

        // ── Grant all permissions to the SystemSuperAdmin role ──
        var superAdminRoleKey = systemSuperAdminRoleId.ToString();
        foreach (var def in defEntities)
        {
            var existing = await permGrantRepo.FindAsync(def.Name, "R", superAdminRoleKey);
            if (existing is null)
            {
                var grant = new IdentityPermissionGrant(
                    Guid.CreateVersion7(), def.Name, "R", superAdminRoleKey);
                await permGrantRepo.InsertAsync(grant);
            }
        }
        logger.LogInformation("All permissions granted to 'SystemSuperAdmin' role.");

        // ── Grant specific permissions to the TenantAdmin role ──
        // Only grant permissions that belong to Tenant side (Both or Tenant)
        var tenantAdminRoleKey = tenantAdminRoleId.ToString();
        var tenantSidePermissions = defEntities
            .Where(d => d.MultiTenancySide == MultiTenancySides.Both || d.MultiTenancySide == MultiTenancySides.Tenant)
            .ToList();

        foreach (var def in tenantSidePermissions)
        {
            var existing = await permGrantRepo.FindAsync(def.Name, "R", tenantAdminRoleKey);
            if (existing is null)
            {
                var grant = new IdentityPermissionGrant(
                    Guid.CreateVersion7(), def.Name, "R", tenantAdminRoleKey);
                await permGrantRepo.InsertAsync(grant);
            }
        }
        logger.LogInformation("Tenant-specific permissions granted to 'TenantAdmin' role.");

        await uow.CompleteAsync();
    }
}

logger.LogInformation("All Account Service seeding complete.");

public class NullClientInfoProvider : IClientInfoProvider
{
    public string? ClientIpAddress => null;
    public string? BrowserInfo => null;
    public string? CorrelationId => null;
}
