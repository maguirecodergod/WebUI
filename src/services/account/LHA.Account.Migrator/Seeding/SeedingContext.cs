namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Carries shared state produced during seeding that later seeders depend on.
/// Acts as a lightweight pass-through object between ordered seeding steps,
/// avoiding tight coupling between individual seeder classes.
/// </summary>
internal sealed class SeedingContext
{
    /// <summary>ID of the SystemSuperAdmin role, set by <see cref="IdentityRoleSeeder"/>.</summary>
    public Guid SystemSuperAdminRoleId { get; set; }

    /// <summary>ID of the TenantAdmin role, set by <see cref="IdentityRoleSeeder"/>.</summary>
    public Guid TenantAdminRoleId { get; set; }
}
