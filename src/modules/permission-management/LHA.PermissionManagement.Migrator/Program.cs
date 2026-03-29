using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionTemplates;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(AppContext.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddPermissionManagementEntityFrameworkCore(options =>
{
    options.Configure<PermissionManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("PermissionManagement.Migrator");

// ── 1. Apply pending migrations ──────────────────────────────────
logger.LogInformation("Applying PermissionManagement migrations...");

using (var migrationScope = host.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider
        .GetRequiredService<PermissionManagementDbContext>();
    await dbContext.Database.MigrateAsync();
}

logger.LogInformation("PermissionManagement migrations applied successfully.");

// ── 2. Seed default permission definitions ───────────────────────
logger.LogInformation("Seeding default data...");

using (var seedScope = host.Services.CreateScope())
{
    var uowManager = seedScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
    var defRepo = seedScope.ServiceProvider.GetRequiredService<IPermissionDefinitionRepository>();
    var groupRepo = seedScope.ServiceProvider.GetRequiredService<IPermissionGroupRepository>();
    var templateRepo = seedScope.ServiceProvider.GetRequiredService<IPermissionTemplateRepository>();

    using var uow = uowManager.Begin(isTransactional: true);

    // Seed identity permissions
    var identityPermissions = new (string Name, string DisplayName, string GroupName)[]
    {
        ("identity.users.read", "View Users", "UserManagement"),
        ("identity.users.create", "Create Users", "UserManagement"),
        ("identity.users.update", "Update Users", "UserManagement"),
        ("identity.users.delete", "Delete Users", "UserManagement"),
        ("identity.roles.read", "View Roles", "RoleManagement"),
        ("identity.roles.create", "Create Roles", "RoleManagement"),
        ("identity.roles.update", "Update Roles", "RoleManagement"),
        ("identity.roles.delete", "Delete Roles", "RoleManagement"),
    };

    var defEntities = new List<PermissionDefinitionEntity>();
    foreach (var (name, displayName, groupName) in identityPermissions)
    {
        var existing = await defRepo.FindByNameAsync(name);
        if (existing is null)
        {
            var def = new PermissionDefinitionEntity(
                Guid.NewGuid(), name, displayName, "Identity", groupName, null);
            await defRepo.InsertAsync(def);
            defEntities.Add(def);
            logger.LogInformation("Permission '{Name}' created.", name);
        }
        else
        {
            defEntities.Add(existing);
        }
    }

    // Seed tenant management permissions
    var tenantPermissions = new (string Name, string DisplayName, string GroupName)[]
    {
        ("tenants.read", "View Tenants", "TenantManagement"),
        ("tenants.create", "Create Tenants", "TenantManagement"),
        ("tenants.update", "Update Tenants", "TenantManagement"),
        ("tenants.delete", "Delete Tenants", "TenantManagement"),
    };

    foreach (var (name, displayName, groupName) in tenantPermissions)
    {
        var existing = await defRepo.FindByNameAsync(name);
        if (existing is null)
        {
            var def = new PermissionDefinitionEntity(
                Guid.NewGuid(), name, displayName, "TenantManagement", groupName, null);
            await defRepo.InsertAsync(def);
            defEntities.Add(def);
            logger.LogInformation("Permission '{Name}' created.", name);
        }
        else
        {
            defEntities.Add(existing);
        }
    }

    // Seed permission groups
    var userMgmtGroup = await groupRepo.FindByNameAsync("UserManagement");
    if (userMgmtGroup is null)
    {
        userMgmtGroup = new PermissionGroupEntity(
            Guid.NewGuid(), "UserManagement", "User Management", "Identity", null);
        foreach (var def in defEntities.Where(d => d.GroupName == "UserManagement"))
            userMgmtGroup.AddPermission(def.Id);
        await groupRepo.InsertAsync(userMgmtGroup);
        logger.LogInformation("Permission group 'UserManagement' created.");
    }

    var roleMgmtGroup = await groupRepo.FindByNameAsync("RoleManagement");
    if (roleMgmtGroup is null)
    {
        roleMgmtGroup = new PermissionGroupEntity(
            Guid.NewGuid(), "RoleManagement", "Role Management", "Identity", null);
        foreach (var def in defEntities.Where(d => d.GroupName == "RoleManagement"))
            roleMgmtGroup.AddPermission(def.Id);
        await groupRepo.InsertAsync(roleMgmtGroup);
        logger.LogInformation("Permission group 'RoleManagement' created.");
    }

    var tenantMgmtGroup = await groupRepo.FindByNameAsync("TenantManagement");
    if (tenantMgmtGroup is null)
    {
        tenantMgmtGroup = new PermissionGroupEntity(
            Guid.NewGuid(), "TenantManagement", "Tenant Management", "TenantManagement", null);
        foreach (var def in defEntities.Where(d => d.GroupName == "TenantManagement"))
            tenantMgmtGroup.AddPermission(def.Id);
        await groupRepo.InsertAsync(tenantMgmtGroup);
        logger.LogInformation("Permission group 'TenantManagement' created.");
    }

    // Seed permission templates
    var adminTemplate = await templateRepo.FindByNameAsync("TenantAdmin");
    if (adminTemplate is null)
    {
        adminTemplate = new PermissionTemplateEntity(
            Guid.NewGuid(), "TenantAdmin", "Tenant Administrator",
            "Full access to all identity and tenant management features.");
        adminTemplate.AddGroup(userMgmtGroup.Id);
        adminTemplate.AddGroup(roleMgmtGroup.Id);
        adminTemplate.AddGroup(tenantMgmtGroup.Id);
        await templateRepo.InsertAsync(adminTemplate);
        logger.LogInformation("Permission template 'TenantAdmin' created.");
    }

    await uow.CompleteAsync();
}

logger.LogInformation("Seeding complete.");
