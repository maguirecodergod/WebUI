using System.Data.Common;
using LHA.Account.Domain.TenantProvisioning;
using LHA.TenantManagement.Domain;
using LHA.Shared.Domain.TenantManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Application.TenantProvisioning.Strategies;

/// <summary>
/// Provisioning strategy for Per-Schema isolation (Shared DB, Dedicated Schema).
/// Appends SearchPath to the connection string and runs migrations.
/// </summary>
public sealed class PerSchemaTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    private readonly ITenantDatabaseMigrator _databaseMigrator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PerSchemaTenantProvisionerStrategy> _logger;

    public CMultiTenancyDatabaseStyle Style => CMultiTenancyDatabaseStyle.PerSchema;

    public PerSchemaTenantProvisionerStrategy(
        ITenantDatabaseMigrator databaseMigrator,
        IConfiguration configuration,
        ILogger<PerSchemaTenantProvisionerStrategy> logger)
    {
        _databaseMigrator = databaseMigrator;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Provisioning dedicated schema for tenant {TenantId} ({TenantName}).", 
            tenant.Id, tenant.Name);

        // 1. Locate base connection string
        var baseConnString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(baseConnString))
        {
            throw new InvalidOperationException("Default connection string is missing.");
        }

        // 2. Generate schema name
        var schemaName = $"tenant_{tenant.NormalizedName.ToLowerInvariant()}";
        
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = baseConnString
        };

        // Append SearchPath for PostgreSQL schema isolation
        builder["SearchPath"] = schemaName;
        var newConnString = builder.ToString();

        // 3. Register string inside Tenant
        tenant.AddOrUpdateConnectionString(TenantConsts.DefaultConnectionStringName, newConnString);

        // 4. Trigger database migration locally
        // The implementation in EntityFrameworkCore will handle schema creation if SearchPath is detected.
        try
        {
            await _databaseMigrator.MigrateAsync(newConnString, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations to tenant schema {SchemaName}", schemaName);
            throw;
        }

        _logger.LogInformation("Dedicated schema {SchemaName} successfully provisioned.", schemaName);
    }
}
