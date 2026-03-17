using LHA.Core;
using LHA.Ddd.Application;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.Shared.Contracts.TenantManagement;

/// <summary>
/// Full tenant DTO with audit fields and connection strings.
/// </summary>
public sealed class TenantDto : FullAuditedEntityDto<Guid>
{
    public required string Name { get; init; }
    public CMasterStatus Status { get; init; }
    public MultiTenancyDatabaseStyle DatabaseStyle { get; init; }
    public string ConcurrencyStamp { get; init; } = string.Empty;
    public List<TenantConnectionStringDto> ConnectionStrings { get; init; } = [];
}

/// <summary>
/// Represents a single named connection string of a tenant.
/// </summary>
public sealed class TenantConnectionStringDto
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
