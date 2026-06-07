using LHA.Core;
using LHA.Ddd.Application;
using LHA.Shared.Domain.TenantManagement;

namespace LHA.Shared.Contracts.TenantManagement;

/// <summary>
/// Full tenant DTO with audit fields and connection strings.
/// </summary>
public sealed class TenantDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// Unique tenant name used for identification and subdomain routing.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Current lifecycle status of the tenant (e.g., Active, Suspended).
    /// </summary>
    public CMasterStatus Status { get; init; }

    /// <summary>
    /// Database isolation strategy for this tenant (e.g., Shared, Dedicated).
    /// </summary>
    public CMultiTenancyDatabaseStyle DatabaseStyle { get; init; }

    /// <summary>
    /// Concurrency stamp used for optimistic locking on tenant updates.
    /// </summary>
    public string ConcurrencyStamp { get; init; } = string.Empty;

    /// <summary>
    /// Collection of named connection strings associated with the tenant.
    /// </summary>
    public List<TenantConnectionStringDto> ConnectionStrings { get; init; } = [];
}

/// <summary>
/// Represents a single named connection string of a tenant.
/// </summary>
public sealed class TenantConnectionStringDto
{
    /// <summary>
    /// Logical name of the connection string (e.g., <c>Default</c>).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Full connection string value.
    /// </summary>
    public required string Value { get; init; }
}
