using LHA.MultiTenancy.Provisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.EntityFrameworkCore.Provisioning;

/// <summary>
/// Generic EF Core implementation of the tenant database migrator. 
/// It connects using the specific connection string and runs Migrations for the specified DbContext.
/// </summary>
public class EfCoreTenantDatabaseMigrator<TDbContext> : ITenantDatabaseMigrator
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EfCoreTenantDatabaseMigrator<TDbContext>> _logger;

    public EfCoreTenantDatabaseMigrator(
        IServiceProvider serviceProvider, 
        ILogger<EfCoreTenantDatabaseMigrator<TDbContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database migration for new tenant database (Context: {ContextType})...", typeof(TDbContext).Name);

        // Use a new DI scope so we don't interfere with current request's DbContext
        using var scope = _serviceProvider.CreateScope();

        // Build options strictly for this dynamic connection string
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Required dependencies for the DbContext constructor
        // By resolving the context through DI but with overridden options, we ensure it gets its usual dependencies.
        // Wait, DbContext constructors often take DbContextOptions<TContext>, so we can instantiate it dynamically
        // but if it has other dependencies, we need ActivatorUtilities to inject them.
        var dbContext = ActivatorUtilities.CreateInstance<TDbContext>(scope.ServiceProvider, optionsBuilder.Options);

        try
        {
            // ── PostgreSQL Schema Isolation Support ───────────────────────
            // If the connection string specifies a SearchPath, ensure that schema exists first.
            var resolvedConnectionString = dbContext.Database.GetConnectionString();
            if (!string.IsNullOrWhiteSpace(resolvedConnectionString) && resolvedConnectionString.Contains("SearchPath=", StringComparison.OrdinalIgnoreCase))
            {
                var builder = new Npgsql.NpgsqlConnectionStringBuilder(resolvedConnectionString);
                if (!string.IsNullOrWhiteSpace(builder.SearchPath))
                {
                    // Escape double quotes to prevent SQL injection in schema name
                    var safeSchemaName = builder.SearchPath.Replace("\"", "\"\"");
                    
                    // We create a separate connection to avoid interfering with the DbContext's state.
#pragma warning disable EF1002 // Vulnerable to SQL injection
                    await dbContext.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS \"" + safeSchemaName + "\";", cancellationToken);
#pragma warning restore EF1002 // Vulnerable to SQL injection
                }
            }

            // Executes schema creation (creates DB if not exists contextually via the connection string)
            await dbContext.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Successfully migrated new tenant database/schema (Context: {ContextType}).", typeof(TDbContext).Name);
        }
        finally
        {
            await dbContext.DisposeAsync();
        }
    }
}
