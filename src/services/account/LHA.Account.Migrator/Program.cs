using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.MultiTenancy;
using LHA.MultiTenancy.Provisioning;
using LHA.TenantManagement.Domain;
using LHA.TenantManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// -- Minimal framework services ----------------------------------------
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHAAuditLogging();

// -- Only EntityFrameworkCore modules (no Application layer) ---------
builder.Services.AddAccountEntityFrameworkCore(connectionString);
builder.Services.AddTenantManagementEntityFrameworkCore(options =>
{
    options.Configure<TenantManagementDbContext>(ctx =>
    {
        ctx.DbContextOptions.UseNpgsql(connectionString);
        // Suppress pending model changes warning for migration tool
        ctx.DbContextOptions.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    });
});
builder.Services.AddTransient<TenantManager>();

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Account.Migrator");

try
{
    // -- 1. Migrate Account database ------------------------------------
    logger.LogInformation("Migrating Account database...");
    using (var scope = host.Services.CreateScope())
    {
        var accountDbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await accountDbContext.Database.MigrateAsync();
    }
    logger.LogInformation("Account database migrated successfully.");

    // -- 2. Migrate TenantManagement database ---------------------------
    // NOTE: Skip if there are pending migration conflicts
    // logger.LogInformation("Migrating TenantManagement database...");
    // using (var scope = host.Services.CreateScope())
    // {
    //     var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantManagementDbContext>();
    //     await tenantDbContext.Database.MigrateAsync();
    // }
    // logger.LogInformation("TenantManagement database migrated successfully.");

    // -- 3. Migrate all tenant databases ---------------------------------
    logger.LogInformation("Migrating all tenant databases...");

    using (var scope = host.Services.CreateScope())
    {
        var migrators = scope.ServiceProvider.GetServices<ITenantDatabaseMigrator>();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        if (!migrators.Any())
        {
            logger.LogWarning("No ITenantDatabaseMigrator registered. Skipping tenant database migration.");
        }
        else
        {
            // Need UnitOfWork for repository calls
            using var uow = uowManager.Begin(isTransactional: false);

            var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var tenants = await tenantRepo.GetTenantWithConnectionStringAsync();

            if (tenants.Count == 0)
            {
                logger.LogInformation("No tenants found to migrate.");
            }
            else
            {
                logger.LogInformation("Found {Count} tenants. Starting tenant database migration...", tenants.Count);

                var successCount = 0;
                var failureCount = 0;

                foreach (var tenant in tenants)
                {
                    try
                    {
                        var tenantConnString = tenant.ConnectionStrings.FirstOrDefault()?.Value
                            ?? throw new InvalidOperationException($"Tenant '{tenant.Name}' has no connection string.");

                        logger.LogInformation("Migrating database for tenant {TenantName}...", tenant.Name);

                        foreach (var migrator in migrators)
                        {
                            try
                            {
                                await migrator.MigrateAsync(tenantConnString);
                                logger.LogInformation("✓ Migrated {MigratorType} for tenant {TenantName}",
                                    migrator.GetType().Name, tenant.Name);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "✗ Failed to migrate {MigratorType} for tenant {TenantName}",
                                    migrator.GetType().Name, tenant.Name);
                                throw;
                            }
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failureCount++;
                        logger.LogError(ex, "✗ Failed to migrate tenant {TenantName}", tenant.Name);
                    }
                }

                logger.LogInformation("Tenant database migration complete. Success: {Success}, Failed: {Failure}",
                    successCount, failureCount);

                if (failureCount > 0)
                {
                    throw new InvalidOperationException($"Failed to migrate {failureCount} tenant database(s).");
                }
            }
        }
    }

    logger.LogInformation("✓ All migrations completed successfully!");
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Migration failed!");
    Environment.Exit(1);
}
