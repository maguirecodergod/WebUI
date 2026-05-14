using LHA.MultiTenancy.Provisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace LHA.EntityFrameworkCore.MongoDB.Provisioning;

/// <summary>
/// Generic EF Core implementation of the tenant database migrator for MongoDB. 
/// It connects using the specific connection string and ensures the database is created.
/// </summary>
public class MongoDbTenantDatabaseMigrator<TDbContext> : ITenantDatabaseMigrator
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MongoDbTenantDatabaseMigrator<TDbContext>> _logger;

    public MongoDbTenantDatabaseMigrator(
        IServiceProvider serviceProvider, 
        ILogger<MongoDbTenantDatabaseMigrator<TDbContext>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting database initialization for new tenant database (Context: {ContextType})...", typeof(TDbContext).Name);

        // Use a new DI scope so we don't interfere with current request's DbContext
        using var scope = _serviceProvider.CreateScope();

        // Build options strictly for this dynamic connection string
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        
        var mongoUrl = new MongoUrl(connectionString);
        var client = new MongoClient(mongoUrl);
        var databaseName = mongoUrl.DatabaseName ?? "default_db";
        
        optionsBuilder.UseMongoDB(client, databaseName);

        // Required dependencies for the DbContext constructor
        var dbContext = ActivatorUtilities.CreateInstance<TDbContext>(scope.ServiceProvider, optionsBuilder.Options);

        try
        {
            // MongoDB does not support relational migrations.
            // EnsureCreatedAsync ensures that collections exist according to the model.
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);

            _logger.LogInformation("Successfully initialized new tenant database (Context: {ContextType}).", typeof(TDbContext).Name);
        }
        finally
        {
            await dbContext.DisposeAsync();
        }
    }
}
