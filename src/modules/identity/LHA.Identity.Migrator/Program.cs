using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Identity.Application;
using LHA.Identity.Domain;
using LHA.Identity.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityEntityFrameworkCore(options =>
{
    options.Configure<IdentityDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Identity.Migrator");

// ── 1. Apply pending migrations ──────────────────────────────────
logger.LogInformation("Applying Identity migrations...");

using (var migrationScope = host.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await dbContext.Database.MigrateAsync();
}

logger.LogInformation("Identity migrations applied successfully.");

// ── 2. Seed default data ─────────────────────────────────────────
logger.LogInformation("Seeding default Identity data...");

using (var seedScope = host.Services.CreateScope())
{
    var uowManager = seedScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
    var roleManager = seedScope.ServiceProvider.GetRequiredService<IdentityRoleManager>();
    var roleRepository = seedScope.ServiceProvider.GetRequiredService<IIdentityRoleRepository>();
    var userManager = seedScope.ServiceProvider.GetRequiredService<IdentityUserManager>();
    var userRepository = seedScope.ServiceProvider.GetRequiredService<IIdentityUserRepository>();

    using var uow = uowManager.Begin(isTransactional: true);

    // ── Seed "admin" role ────────────────────────────────────────
    var adminRoleName = "admin";
    var existingRole = await roleRepository.FindByNormalizedNameAsync(adminRoleName.ToUpperInvariant());

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

    // ── Seed "admin" user ────────────────────────────────────────
    var adminUserName = "admin";
    var existingUser = await userRepository.FindByNormalizedUserNameAsync(adminUserName.ToUpperInvariant());

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

logger.LogInformation("Seeding complete.");
