namespace LHA.Shared.Domain.TenantManagement;

/// <summary>
/// Determines how a tenant's data is isolated at the database level.
/// </summary>
[Flags]
public enum CMultiTenancyDatabaseStyle
{
    /// <summary>
    /// 1 - Shared: All tenants share a single database, isolated by TenantId column.
    /// </summary>
    Shared = 1,

    /// <summary>
    /// 2 - PerTenant: Each tenant has its own dedicated database.
    /// </summary>
    PerTenant = 2,

    /// <summary>
    /// 4 - PerSchema: Each tenant has its own dedicated schema within a shared database.
    /// </summary>
    PerSchema = 4,

    /// <summary>Mix of shared and per-tenant — some tenants have dedicated databases, others share.</summary>
    Hybrid = Shared | PerTenant | PerSchema
}
