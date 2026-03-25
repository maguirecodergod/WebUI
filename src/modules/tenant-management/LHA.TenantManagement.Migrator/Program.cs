using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.TenantManagement.Domain;
using LHA.TenantManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// -- Framework services -------------------------------------------
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// -- Module services ----------------------------------------------
builder.Services.AddTenantManagementEntityFrameworkCore(options =>
{
    options.Configure<TenantManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});
builder.Services.AddTransient<TenantManager>();

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("TenantManagement.Migrator");

// -- 1. Apply pending migrations ----------------------------------
logger.LogInformation("Applying TenantManagement migrations...");

using (var migrationScope = host.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();
    await dbContext.Database.MigrateAsync();
}

logger.LogInformation("TenantManagement migrations applied successfully.");

// -- 2. Seed default data -----------------------------------------
logger.LogInformation("Seeding default data...");

using (var seedScope = host.Services.CreateScope())
{
    var uowManager = seedScope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
    var tenantManager = seedScope.ServiceProvider.GetRequiredService<TenantManager>();
    var tenantRepo = seedScope.ServiceProvider.GetRequiredService<ITenantRepository>();

    using var uow = uowManager.Begin(isTransactional: true);

    var existing = await tenantRepo.FindByNameAsync("DEFAULT");

    if (existing is null)
    {
        var tenant = await tenantManager.CreateAsync("Default");
        await tenantRepo.InsertAsync(tenant);
        logger.LogInformation("Default tenant '{TenantName}' created.", tenant.Name);
    }
    else
    {
        logger.LogInformation("Default tenant already exists, skipping seed.");
    }

    await uow.CompleteAsync();
}

logger.LogInformation("Seeding complete.");
