using LHA.Account.Domain.TenantProvisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.EntityFrameworkCore.TenantProvisioning;

/// <summary>
/// EF Core implementation of the tenant database migrator. 
/// It connects using the specific connection string and runs Migrations.
/// </summary>
public sealed class TenantDatabaseMigrator : ITenantDatabaseMigrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantDatabaseMigrator> _logger;

    public TenantDatabaseMigrator(
        IServiceProvider serviceProvider, 
        ILogger<TenantDatabaseMigrator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database migration for new tenant database...");

        // Use a new DI scope so we don't interfere with current request's DbContext
        using var scope = _serviceProvider.CreateScope();

        // Build options strictly for this dynamic connection string
        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Required dependencies for the AccountDbContext constructor
        var auditPropertySetter = scope.ServiceProvider.GetService<LHA.Auditing.IAuditPropertySetter>();
        var currentTenant = scope.ServiceProvider.GetService<LHA.MultiTenancy.ICurrentTenant>();

        // Create the composite Database Context exclusively to apply schema
        await using var dbContext = new AccountDbContext(
            optionsBuilder.Options,
            scope.ServiceProvider,
            auditPropertySetter,
            currentTenant);

        // Executes schema creation (creates DB if not exists contextually via the connection string)
        await dbContext.Database.MigrateAsync(cancellationToken);

        _logger.LogInformation("Successfully migrated new tenant database.");
    }
}
