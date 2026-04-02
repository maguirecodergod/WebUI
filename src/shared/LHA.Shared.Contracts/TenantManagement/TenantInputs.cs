using LHA.Core;
using LHA.Ddd.Application;
using LHA.Shared.Domain.TenantManagement;
// using LHA.TenantManagement.Domain.Shared;

namespace LHA.Shared.Contracts.TenantManagement;

/// <summary>
/// Input for creating a new tenant.
/// </summary>
public sealed class CreateTenantInput
{
    /// <summary>Display name (must be unique).</summary>
    public required string Name { get; init; }

    /// <summary>Database isolation strategy. Defaults to <see cref="CMultiTenancyDatabaseStyle.Shared"/>.</summary>
    public CMultiTenancyDatabaseStyle DatabaseStyle { get; init; } = CMultiTenancyDatabaseStyle.Shared;

    /// <summary>
    /// Initial connection strings keyed by logical name (e.g., "Default").
    /// </summary>
    public Dictionary<string, string> ConnectionStrings { get; init; } = [];
}

/// <summary>
/// Input for updating an existing tenant.
/// Includes <see cref="ConcurrencyStamp"/> for optimistic concurrency.
/// </summary>
public sealed class UpdateTenantInput
{
    /// <summary>New display name.</summary>
    public required string Name { get; init; }

    /// <summary>
    /// Concurrency stamp obtained from the last read.
    /// Used for optimistic-concurrency conflict detection.
    /// </summary>
    public required string ConcurrencyStamp { get; init; }
}

/// <summary>
/// Input for querying tenants with filtering and paging.
/// </summary>
public sealed class GetTenantsInput : PagedAndSortedResultRequestDto
{
    /// <summary>Optional name filter (contains, case-insensitive).</summary>
    public string? Filter { get; init; }

    /// <summary>Filter by status. <c>null</c> returns all.</summary>
    public CMasterStatus? Status { get; init; }
}

/// <summary>
/// Input for setting a connection string on a tenant.
/// </summary>
public sealed class SetConnectionStringInput
{
    /// <summary>The connection string value.</summary>
    public required string Value { get; init; }
}
