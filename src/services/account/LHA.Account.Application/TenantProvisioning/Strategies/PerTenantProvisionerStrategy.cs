using System.Data.Common;
using LHA.Account.Domain.TenantProvisioning;
using LHA.TenantManagement.Domain;
using LHA.Shared.Domain.TenantManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Application.TenantProvisioning.Strategies;

/// <summary>
/// Provisioning strategy for Dedicated Per-Tenant DBs.
/// Generates connection strings and runs EF Core migrations for the new database.
/// </summary>
public sealed class PerTenantProvisionerStrategy : ITenantProvisionerStrategy
{
    private readonly ITenantDatabaseMigrator _databaseMigrator;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PerTenantProvisionerStrategy> _logger;

    public CMultiTenancyDatabaseStyle Style => CMultiTenancyDatabaseStyle.PerTenant;

    public PerTenantProvisionerStrategy(
        ITenantDatabaseMigrator databaseMigrator,
        IConfiguration configuration,
        ILogger<PerTenantProvisionerStrategy> logger)
    {
        _databaseMigrator = databaseMigrator;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProvisionAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Provisioning dedicated database for tenant {TenantId} ({TenantName}).", 
            tenant.Id, tenant.Name);

        // 1. Locate template connection string (Assume host 'Default')
        var baseConnString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(baseConnString))
        {
            throw new InvalidOperationException("Default connection string is missing for generating tenant DB template.");
        }

        // 2. Generate a unique database name utilizing the connection builder
        var dbName = $"LHA_Tenant_{tenant.NormalizedName}";
        
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = baseConnString
        };

        // Standard Postgres database mapping field
        builder["Database"] = dbName;
        var newConnString = builder.ToString();

        // 3. Register string inside Tenant
        tenant.AddOrUpdateConnectionString(TenantConsts.DefaultConnectionStringName, newConnString);

        // 4. Trigger database migration locally
        try
        {
            await _databaseMigrator.MigrateAsync(newConnString, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply migrations to tenant database {DbName}", dbName);
            // Re-throw so upstream components (Auth service) know provisioning failed
            throw;
        }

        _logger.LogInformation("Dedicated database {DbName} successfully provisioned.", dbName);
    }
}
