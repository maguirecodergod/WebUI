using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LHA.MultiTenancy.Provisioning.Strategies;

/// <summary>
/// Provisioning strategy for Dedicated Per-Tenant DBs.
/// Generates connection strings and runs EF Core migrations for the new database.
/// </summary>
public sealed class PerTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    private readonly IEnumerable<ITenantDatabaseMigrator> _databaseMigrators;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PerTenantProvisionerStrategy> _logger;

    public int Style => 2; // PerTenant

    public PerTenantProvisionerStrategy(
        IEnumerable<ITenantDatabaseMigrator> databaseMigrators,
        IConfiguration configuration,
        ILogger<PerTenantProvisionerStrategy> logger)
    {
        _databaseMigrators = databaseMigrators;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Provisioning dedicated database for tenant {TenantId} ({TenantName}).", 
            tenantId, normalizedTenantName);

        // 1. Locate template connection string (Assume host 'Default')
        var baseConnString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(baseConnString))
        {
            throw new InvalidOperationException("Default connection string is missing for generating tenant DB template.");
        }

        // 2. Generate a unique database name utilizing the connection builder
        var dbName = $"LHA_Tenant_{normalizedTenantName}";
        
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = baseConnString
        };

        // Standard Postgres database mapping field
        builder["Database"] = dbName;
        var newConnString = builder.ToString();

        // 3. Trigger database migration locally for all registered migrators
        foreach (var migrator in _databaseMigrators)
        {
            try
            {
                await migrator.MigrateAsync(newConnString, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply migrations to tenant database {DbName} using migrator {MigratorType}", 
                    dbName, migrator.GetType().Name);
                // Re-throw so upstream components know provisioning failed
                throw;
            }
        }

        _logger.LogInformation("Dedicated database {DbName} successfully provisioned.", dbName);
        
        return newConnString;
    }
}
