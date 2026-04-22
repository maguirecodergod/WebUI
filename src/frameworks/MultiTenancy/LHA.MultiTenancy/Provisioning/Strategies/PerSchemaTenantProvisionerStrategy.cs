using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LHA.MultiTenancy.Provisioning.Strategies;

/// <summary>
/// Provisioning strategy for Per-Schema isolation (Shared DB, Dedicated Schema).
/// Appends SearchPath to the connection string and runs migrations for all registered contexts.
/// </summary>
public sealed class PerSchemaTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    private readonly IEnumerable<ITenantDatabaseMigrator> _databaseMigrators;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PerSchemaTenantProvisionerStrategy> _logger;

    public int Style => 4; // PerSchema

    public PerSchemaTenantProvisionerStrategy(
        IEnumerable<ITenantDatabaseMigrator> databaseMigrators,
        IConfiguration configuration,
        ILogger<PerSchemaTenantProvisionerStrategy> logger)
    {
        _databaseMigrators = databaseMigrators;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> ProvisionAsync(Guid tenantId, string normalizedTenantName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Provisioning dedicated schema for tenant {TenantId} ({TenantName}).", 
            tenantId, normalizedTenantName);

        // 1. Locate base connection string
        var baseConnString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(baseConnString))
        {
            throw new InvalidOperationException("Default connection string is missing.");
        }

        // 2. Generate schema name
        var schemaName = $"tenant_{normalizedTenantName.ToLowerInvariant()}";
        
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = baseConnString
        };

        // Append SearchPath for PostgreSQL schema isolation
        builder["SearchPath"] = schemaName;
        var newConnString = builder.ToString();

        // 3. Trigger database migration locally for all registered migrators
        // Each migrator will handle schema creation if SearchPath is detected.
        foreach (var migrator in _databaseMigrators)
        {
            try
            {
                await migrator.MigrateAsync(newConnString, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply migrations to tenant schema {SchemaName} using migrator {MigratorType}", 
                    schemaName, migrator.GetType().Name);
                throw;
            }
        }

        _logger.LogInformation("Dedicated schema {SchemaName} successfully provisioned.", schemaName);
        
        // Return the new connection string so the orchestrator can update the tenant
        return newConnString;
    }
}
