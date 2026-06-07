using LHA.Account.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Database;

/// <summary>
/// Applies pending Entity Framework Core migrations against the Account database.
/// </summary>
internal static class DatabaseMigrationRunner
{
    /// <summary>
    /// Creates a short-lived scope, resolves <see cref="AccountDbContext"/>,
    /// and calls <c>MigrateAsync()</c> to apply any pending migrations.
    /// </summary>
    public static async Task RunAsync(IServiceProvider services, ILogger logger)
    {
        logger.LogInformation("Applying Account Service migrations...");

        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Account Service migrations applied successfully.");
    }
}
