namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Contract for a unit of seed logic.
/// Each implementation is responsible for seeding one cohesive domain area
/// (e.g. identity roles, tenants, permissions).
/// </summary>
internal interface IDataSeeder
{
    /// <summary>
    /// Executes the seeding logic using the provided <paramref name="serviceProvider"/>
    /// resolved from the current DI scope.
    /// </summary>
    Task SeedAsync(IServiceProvider serviceProvider, SeedingContext context);
}
