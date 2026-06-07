using LHA.MultiTenancy;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.PermissionManagement;

// ──────────────────────────────────────────────────────────────────
//  PermissionDefinition DTOs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Represents a single permission definition registered by a microservice.
/// </summary>
public sealed class PermissionDefinitionDto : EntityDto<Guid>
{
    /// <summary>
    /// Unique permission name (e.g., <c>identity.users.read</c>).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name for UI rendering.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Name of the owning microservice.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Logical grouping name for organizing related permissions.
    /// </summary>
    public string? GroupName { get; init; }

    /// <summary>
    /// Optional detailed description of what this permission controls.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Indicates whether the permission applies to Host, Tenant, or both.
    /// </summary>
    public CMultiTenancySidesType MultiTenancySide { get; init; } = CMultiTenancySidesType.Both;
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGroup DTOs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Represents a named group of permission definitions.
/// </summary>
public sealed class PermissionGroupDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// Unique group name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Owning microservice name.
    /// </summary>
    public required string ServiceName { get; init; }

    /// <summary>
    /// Optional group description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Permissions belonging to this group.
    /// </summary>
    public List<PermissionDefinitionDto> Permissions { get; init; } = [];
}

// ──────────────────────────────────────────────────────────────────
//  PermissionTemplate DTOs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// A reusable template that bundles permission groups for rapid role setup.
/// </summary>
public sealed class PermissionTemplateDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// Unique template name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Human-readable display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional template description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Permission groups included in this template.
    /// </summary>
    public List<PermissionGroupDto> Groups { get; init; } = [];
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGrant DTOs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Represents a persisted permission grant for a specific provider.
/// </summary>
public sealed class PermissionGrantDto : EntityDto<Guid>
{
    /// <summary>
    /// Tenant scope of the grant. <c>null</c> for host-level grants.
    /// </summary>
    public Guid? TenantId { get; init; }

    /// <summary>
    /// Unique permission name that was granted.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Provider type that holds the grant (e.g., <c>R</c> for Role, <c>U</c> for User).
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Provider-specific key (e.g., role ID or user ID).
    /// </summary>
    public required string ProviderKey { get; init; }
}
