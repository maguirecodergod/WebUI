namespace LHA.MultiTenancy.Provisioning;

/// <summary>
/// Domain-level abstraction for migrating a specific tenant's database schema.
/// This allows different modules to register their own migrators.
/// </summary>
public interface ITenantDatabaseMigrator
{
    /// <summary>
    /// Executes database creation and schema migrations on a specific connection.
    /// </summary>
    Task MigrateAsync(string connectionString, CancellationToken cancellationToken = default);
}
