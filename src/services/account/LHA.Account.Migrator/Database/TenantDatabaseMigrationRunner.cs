using LHA.MultiTenancy;
using LHA.MultiTenancy.Provisioning;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Database;

/// <summary>
/// Migrates all tenant databases using registered ITenantDatabaseMigrators.
/// This ensures that when you add a new migration, all tenant databases are updated.
/// </summary>
internal static class TenantDatabaseMigrationRunner
{
    /// <summary>
    /// Migrates all tenant databases using all registered ITenantDatabaseMigrators.
    /// </summary>
    public static async Task RunAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();

        var migrators = scope.ServiceProvider.GetServices<ITenantDatabaseMigrator>();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();

        if (!migrators.Any())
        {
            logger.LogWarning("No ITenantDatabaseMigrator registered. Skipping tenant database migration.");
            return;
        }

        // Get all tenants with connection strings (excluding soft-deleted)
        var tenants = await tenantRepo.GetListAsync();

        if (tenants.Count == 0)
        {
            logger.LogInformation("No tenants found to migrate.");
            return;
        }

        logger.LogInformation("Found {Count} tenants. Starting tenant database migration...", tenants.Count);

        var successCount = 0;
        var failureCount = 0;

        foreach (var tenant in tenants)
        {
            try
            {
                // Get the connection string for this tenant
                var connectionString = tenant.FindConnectionString("Default")
                    ?? throw new InvalidOperationException($"Tenant '{tenant.Name}' has no connection string.");

                logger.LogInformation("Migrating database for tenant {TenantName} (ID: {TenantId})...",
                    tenant.Name, tenant.Id);

                // Run all migrators for this tenant's database
                foreach (var migrator in migrators)
                {
                    try
                    {
                        await migrator.MigrateAsync(connectionString);
                        logger.LogInformation("✓ Successfully migrated {MigratorType} for tenant {TenantName}",
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
                logger.LogInformation("✓ Successfully migrated all databases for tenant {TenantName}", tenant.Name);
            }
            catch (Exception ex)
            {
                failureCount++;
                logger.LogError(ex, "✗ Failed to migrate database for tenant {TenantName} (ID: {TenantId})",
                    tenant.Name, tenant.Id);
            }
        }

        logger.LogInformation("Tenant database migration complete. Success: {Success}, Failed: {Failure}",
            successCount, failureCount);

        if (failureCount > 0)
        {
            throw new InvalidOperationException($"Failed to migrate {failureCount} tenant database(s). Check logs for details.");
        }
    }
}
