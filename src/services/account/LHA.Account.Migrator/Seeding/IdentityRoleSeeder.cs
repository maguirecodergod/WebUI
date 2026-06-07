using LHA.Core.Users;
using LHA.Identity.Domain;
using LHA.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Seeds the two built-in identity roles (<c>SystemSuperAdmin</c>, <c>TenantAdmin</c>)
/// and the default admin user, then publishes their IDs via <see cref="SeedingContext"/>
/// for downstream seeders (e.g. <see cref="PermissionSeeder"/>).
/// </summary>
internal sealed class IdentityRoleSeeder : IDataSeeder
{
    private readonly ILogger _logger;

    public IdentityRoleSeeder(ILogger logger) => _logger = logger;

    public async Task SeedAsync(IServiceProvider serviceProvider, SeedingContext context)
    {
        var uowManager    = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var roleManager   = serviceProvider.GetRequiredService<IdentityRoleManager>();
        var roleRepo      = serviceProvider.GetRequiredService<IIdentityRoleRepository>();
        var userManager   = serviceProvider.GetRequiredService<IdentityUserManager>();
        var userRepo      = serviceProvider.GetRequiredService<IIdentityUserRepository>();

        using var uow = uowManager.Begin(isTransactional: true);

        var systemSuperAdminRole = await EnsureRoleAsync(
            roleManager, roleRepo,
            CurrentUserDefaults.SystemSuperAdminRoleName,
            "SystemSuperAdmin role");

        var tenantAdminRole = await EnsureRoleAsync(
            roleManager, roleRepo,
            CurrentUserDefaults.TenantAdminRoleName,
            "TenantAdmin role");

        // Publish role IDs so PermissionSeeder can reference them without re-querying.
        context.SystemSuperAdminRoleId = systemSuperAdminRole.Id;
        context.TenantAdminRoleId      = tenantAdminRole.Id;

        await EnsureAdminUserAsync(userManager, userRepo, context);

        await uow.CompleteAsync();
    }

    // ────────────────────────────────────────────────────────────────

    private async Task<IdentityRole> EnsureRoleAsync(
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepo,
        string roleName,
        string logLabel)
    {
        var existing = await roleRepo.FindByNormalizedNameAsync(roleName.ToUpperInvariant());
        if (existing is not null)
            return existing;

        var role = await roleManager.CreateAsync(roleName, isStatic: true, isPublic: true);
        await roleRepo.InsertAsync(role);
        _logger.LogInformation("{Label} created.", logLabel);
        return role;
    }

    private async Task EnsureAdminUserAsync(
        IdentityUserManager userManager,
        IIdentityUserRepository userRepo,
        SeedingContext context)
    {
        var existing = await userRepo.FindByNormalizedUserNameAsync(
            CurrentUserDefaults.AdminUserName.ToUpperInvariant());

        if (existing is not null)
        {
            _logger.LogInformation("Admin user already exists, skipping.");
            return;
        }

        var adminUser = await userManager.CreateAsync(
            CurrentUserDefaults.AdminUserName,
            CurrentUserDefaults.AdminUserEmail,
            CurrentUserDefaults.AdminUserDefaultPassword,
            CurrentUserDefaults.AdminUserId);

        adminUser.AddRole(context.SystemSuperAdminRoleId);
        adminUser.AddRole(context.TenantAdminRoleId);
        await userRepo.InsertAsync(adminUser);

        _logger.LogInformation(
            "Admin user '{UserName}' created (ID: {UserId}).",
            adminUser.UserName, adminUser.Id);
    }
}
