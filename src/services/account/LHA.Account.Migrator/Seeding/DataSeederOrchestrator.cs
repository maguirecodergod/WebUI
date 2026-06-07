using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Runs all <see cref="IDataSeeder"/> implementations in the correct dependency order
/// within a single DI scope.
/// </summary>
/// <remarks>
/// Seeder execution order matters:
/// <list type="number">
///   <item><see cref="IdentityRoleSeeder"/> — must run first to populate role IDs in <see cref="SeedingContext"/>.</item>
///   <item><see cref="TenantSeeder"/> — independent, can run after roles.</item>
///   <item><see cref="PermissionSeeder"/> — depends on role IDs from step 1.</item>
/// </list>
/// </remarks>
internal static class DataSeederOrchestrator
{
    public static async Task RunAsync(IServiceProvider services, ILogger logger)
    {
        logger.LogInformation("Seeding default data...");

        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var seedingContext = new SeedingContext();

        // Order is intentional — see class remarks.
        IDataSeeder[] seeders =
        [
            new IdentityRoleSeeder(logger),
            new TenantSeeder(logger),
            new PermissionSeeder(logger),
        ];

        foreach (var seeder in seeders)
            await seeder.SeedAsync(sp, seedingContext);

        logger.LogInformation("All Account Service seeding complete.");
    }
}
