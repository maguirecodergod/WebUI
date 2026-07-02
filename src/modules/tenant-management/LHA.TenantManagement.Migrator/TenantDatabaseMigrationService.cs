using LHA.MultiTenancy.Provisioning;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.TenantManagement.Migrator;

/// <summary>
/// Service to migrate all tenant databases using registered ITenantDatabaseMigrators.
/// </summary>
public class TenantDatabaseMigrationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantDatabaseMigrationService> _logger;

    public TenantDatabaseMigrationService(
        IServiceProvider serviceProvider,
        ILogger<TenantDatabaseMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Migrates all tenant databases using all registered ITenantDatabaseMigrators.
    /// </summary>
    public async Task MigrateAllTenantDatabasesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantRepo = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var migrators = scope.ServiceProvider.GetServices<ITenantDatabaseMigrator>();

        if (!migrators.Any())
        {
            _logger.LogWarning("No ITenantDatabaseMigrator registered. Skipping tenant database migration.");
            return;
        }

        // Get all tenants with connection strings (excluding soft-deleted)
        var tenants = await tenantRepo.GetTenantWithConnectionStringAsync(cancellationToken: cancellationToken);

        if (tenants.Count == 0)
        {
            _logger.LogInformation("No tenants found to migrate.");
            return;
        }

        _logger.LogInformation("Found {Count} tenants. Starting migration...", tenants.Count);

        var successCount = 0;
        var failureCount = 0;

        foreach (var tenant in tenants)
        {
            try
            {
                // Get the connection string for this tenant
                var connectionString = tenant.FindConnectionString("Default")
                    ?? throw new InvalidOperationException($"Tenant '{tenant.Name}' has no connection string.");

                _logger.LogInformation("Migrating database for tenant {TenantName} (ID: {TenantId})...",
                    tenant.Name, tenant.Id);

                // Run all migrators for this tenant's database
                foreach (var migrator in migrators)
                {
                    try
                    {
                        await migrator.MigrateAsync(connectionString, cancellationToken);
                        _logger.LogInformation("✓ Successfully migrated {MigratorType} for tenant {TenantName}",
                            migrator.GetType().Name, tenant.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "✗ Failed to migrate {MigratorType} for tenant {TenantName}",
                            migrator.GetType().Name, tenant.Name);
                        throw; // Re-throw to mark tenant as failed
                    }
                }

                successCount++;
                _logger.LogInformation("✓ Successfully migrated all databases for tenant {TenantName}", tenant.Name);
            }
            catch (Exception ex)
            {
                failureCount++;
                _logger.LogError(ex, "✗ Failed to migrate database for tenant {TenantName} (ID: {TenantId})",
                    tenant.Name, tenant.Id);
            }
        }

        _logger.LogInformation("Migration complete. Success: {Success}, Failed: {Failure}",
            successCount, failureCount);

        if (failureCount > 0)
        {
            throw new InvalidOperationException($"Failed to migrate {failureCount} tenant database(s). Check logs for details.");
        }
    }
}
