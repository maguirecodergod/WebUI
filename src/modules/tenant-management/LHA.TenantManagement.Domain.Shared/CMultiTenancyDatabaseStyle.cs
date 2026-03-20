namespace LHA.TenantManagement.Domain.Shared;

/// <summary>
/// Determines how a tenant's data is isolated at the database level.
/// </summary>
[Flags]
public enum CMultiTenancyDatabaseStyle
{
    /// <summary>All tenants share a single database, isolated by TenantId column.</summary>
    Shared = 1,

    /// <summary>Each tenant has its own dedicated database.</summary>
    PerTenant = 2,

    /// <summary>Mix of shared and per-tenant — some tenants have dedicated databases, others share.</summary>
    Hybrid = Shared | PerTenant
}
