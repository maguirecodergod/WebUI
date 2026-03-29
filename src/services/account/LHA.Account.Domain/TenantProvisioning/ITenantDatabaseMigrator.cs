namespace LHA.Account.Domain.TenantProvisioning;

/// <summary>
/// Domain-level abstraction for migrating a specific tenant's database schema.
/// This hides the EF Core execution logic from the Application layer.
/// </summary>
public interface ITenantDatabaseMigrator
{
    /// <summary>
    /// Executes database creation and schema migrations on a specific connection.
    /// </summary>
    Task MigrateAsync(string connectionString, CancellationToken cancellationToken = default);
}
