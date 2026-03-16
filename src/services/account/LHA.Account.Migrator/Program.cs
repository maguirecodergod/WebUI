using LHA.Account.Application;
using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain;
using LHA.TenantManagement.Domain;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

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
    Guid adminRoleId;

    // ── 2a. Identity: admin role + admin user ────────────────────
    var roleManager = scope.ServiceProvider.GetRequiredService<IdentityRoleManager>();
    var roleRepository = scope.ServiceProvider.GetRequiredService<IIdentityRoleRepository>();
    var userManager = scope.ServiceProvider.GetRequiredService<IdentityUserManager>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IIdentityUserRepository>();

    using (var uow = uowManager.Begin(isTransactional: true))
    {
        var adminRoleName = "admin";
        var existingRole = await roleRepository.FindByNormalizedNameAsync(
            adminRoleName.ToUpperInvariant());

        IdentityRole adminRole;
        if (existingRole is null)
        {
            adminRole = await roleManager.CreateAsync(adminRoleName);
            adminRole.SetIsDefault(false);
            await roleRepository.InsertAsync(adminRole);
            logger.LogInformation("Admin role '{RoleName}' created.", adminRole.Name);
        }
        else
        {
            adminRole = existingRole;
            logger.LogInformation("Admin role already exists, skipping.");
        }

        adminRoleId = adminRole.Id;

        var adminUserName = "admin";
        var existingUser = await userRepository.FindByNormalizedUserNameAsync(
            adminUserName.ToUpperInvariant());

        if (existingUser is null)
        {
            var adminUser = await userManager.CreateAsync(
                adminUserName, "admin@lienhoaapp.com", "Admin@123456");
            adminUser.AddRole(adminRole.Id);
            await userRepository.InsertAsync(adminUser);
            logger.LogInformation("Admin user '{UserName}' created.", adminUser.UserName);
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
        var existing = await tenantRepo.FindByNameAsync("DEFAULT");

        if (existing is null)
        {
            var tenant = await tenantManager.CreateAsync("Default");
            await tenantRepo.InsertAsync(tenant);
            logger.LogInformation("Default tenant '{TenantName}' created.", tenant.Name);
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
        var allPermissions = new (string Name, string DisplayName, string Service, string GroupName)[]
        {
            // Identity — Users
            ("identity.users.read",         "View Users",         "Identity", "UserManagement"),
            ("identity.users.create",       "Create Users",       "Identity", "UserManagement"),
            ("identity.users.update",       "Update Users",       "Identity", "UserManagement"),
            ("identity.users.delete",       "Delete Users",       "Identity", "UserManagement"),
            // Identity — Roles
            ("identity.roles.read",         "View Roles",         "Identity", "RoleManagement"),
            ("identity.roles.create",       "Create Roles",       "Identity", "RoleManagement"),
            ("identity.roles.update",       "Update Roles",       "Identity", "RoleManagement"),
            ("identity.roles.delete",       "Delete Roles",       "Identity", "RoleManagement"),
            // Identity — Claim Types
            ("identity.claim-types.read",   "View Claim Types",   "Identity", "ClaimTypeManagement"),
            ("identity.claim-types.create", "Create Claim Types", "Identity", "ClaimTypeManagement"),
            ("identity.claim-types.update", "Update Claim Types", "Identity", "ClaimTypeManagement"),
            ("identity.claim-types.delete", "Delete Claim Types", "Identity", "ClaimTypeManagement"),
            // Identity — Security Logs
            ("identity.security-logs.read", "View Security Logs", "Identity", "SecurityLogManagement"),
            // Tenant Management
            ("tenants.read",                "View Tenants",       "TenantManagement", "TenantManagement"),
            ("tenants.create",              "Create Tenants",     "TenantManagement", "TenantManagement"),
            ("tenants.update",              "Update Tenants",     "TenantManagement", "TenantManagement"),
            ("tenants.delete",              "Delete Tenants",     "TenantManagement", "TenantManagement"),
            // Audit Logs
            ("audit-logs.read",             "View Audit Logs",    "AuditLog", "AuditLogManagement"),
            // Permission Management
            ("permissions.definitions.read",   "View Permission Definitions",   "PermissionManagement", "PermissionMgmt"),
            ("permissions.definitions.manage", "Manage Permission Definitions", "PermissionManagement", "PermissionMgmt"),
            ("permissions.groups.read",        "View Permission Groups",        "PermissionManagement", "PermissionMgmt"),
            ("permissions.groups.manage",      "Manage Permission Groups",      "PermissionManagement", "PermissionMgmt"),
            ("permissions.templates.read",     "View Permission Templates",     "PermissionManagement", "PermissionMgmt"),
            ("permissions.templates.manage",   "Manage Permission Templates",   "PermissionManagement", "PermissionMgmt"),
            ("permissions.grants.read",        "View Permission Grants",        "PermissionManagement", "PermissionMgmt"),
            ("permissions.grants.manage",      "Manage Permission Grants",      "PermissionManagement", "PermissionMgmt"),
        };

        var defEntities = new List<PermissionDefinition>();
        foreach (var (name, displayName, service, groupName) in allPermissions)
        {
            var def = await defRepo.FindByNameAsync(name);
            if (def is null)
            {
                def = new PermissionDefinition(
                    Guid.NewGuid(), name, displayName, service, groupName, null);
                await defRepo.InsertAsync(def);
                logger.LogInformation("Permission '{Name}' created.", name);
            }
            defEntities.Add(def);
        }

        // ── Permission groups ────────────────────────────────────
        var groupDefs = new (string Name, string DisplayName, string Service)[]
        {
            ("UserManagement",       "User Management",        "Identity"),
            ("RoleManagement",       "Role Management",        "Identity"),
            ("ClaimTypeManagement",  "Claim Type Management",  "Identity"),
            ("SecurityLogManagement","Security Log Management", "Identity"),
            ("TenantManagement",     "Tenant Management",      "TenantManagement"),
            ("AuditLogManagement",   "Audit Log Management",   "AuditLog"),
            ("PermissionMgmt",       "Permission Management",  "PermissionManagement"),
        };

        var groupEntities = new List<PermissionGroup>();
        foreach (var (name, displayName, service) in groupDefs)
        {
            var grp = await groupRepo.FindByNameAsync(name);
            if (grp is null)
            {
                grp = new PermissionGroup(Guid.NewGuid(), name, displayName, service, null);
                foreach (var def in defEntities.Where(d => d.GroupName == name))
                    grp.AddPermission(def.Id);
                await groupRepo.InsertAsync(grp);
                logger.LogInformation("Permission group '{Name}' created.", name);
            }
            groupEntities.Add(grp);
        }

        // ── Permission template: TenantAdmin (full access) ──────
        var adminTemplate = await templateRepo.FindByNameAsync("TenantAdmin");
        if (adminTemplate is null)
        {
            adminTemplate = new PermissionTemplate(
                Guid.NewGuid(), "TenantAdmin", "Tenant Administrator",
                "Full access to all management features.");
            foreach (var grp in groupEntities)
                adminTemplate.AddGroup(grp.Id);
            await templateRepo.InsertAsync(adminTemplate);
            logger.LogInformation("Permission template 'TenantAdmin' created.");
        }

        // ── Grant all permissions to the admin role ──────────────
        var adminRoleKey = adminRoleId.ToString();
        foreach (var def in defEntities)
        {
            var existing = await permGrantRepo.FindAsync(def.Name, "R", adminRoleKey);
            if (existing is null)
            {
                var grant = new IdentityPermissionGrant(
                    Guid.CreateVersion7(), def.Name, "R", adminRoleKey);
                await permGrantRepo.InsertAsync(grant);
            }
        }
        logger.LogInformation("All permissions granted to 'admin' role (ID: {RoleId}).", adminRoleId);

        await uow.CompleteAsync();
    }
}

logger.LogInformation("All Account Service seeding complete.");
