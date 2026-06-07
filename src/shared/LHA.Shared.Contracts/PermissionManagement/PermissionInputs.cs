using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.PermissionManagement;

// ──────────────────────────────────────────────────────────────────
//  PermissionDefinition Inputs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Input for creating a new permission definition.
/// </summary>
public sealed class CreatePermissionDefinitionInput
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
}

/// <summary>
/// Input for querying permission definitions with optional filters and pagination.
/// </summary>
public sealed class GetPermissionDefinitionsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Optional text filter to search by permission name or display name.
    /// </summary>
    public string? Filter { get; init; }

    /// <summary>
    /// Optional filter to restrict results to a specific owning microservice.
    /// </summary>
    public string? ServiceName { get; init; }

    /// <summary>
    /// Optional filter to restrict results to a specific permission group.
    /// </summary>
    public string? GroupName { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGroup Inputs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Input for creating a new permission group.
/// </summary>
public sealed class CreatePermissionGroupInput
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
    /// Collection of permission definition IDs to include in this group.
    /// </summary>
    public List<Guid> PermissionDefinitionIds { get; init; } = [];
}

/// <summary>
/// Input for updating an existing permission group.
/// </summary>
public sealed class UpdatePermissionGroupInput
{
    /// <summary>
    /// Updated human-readable display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional updated group description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated collection of permission definition IDs to include in this group.
    /// </summary>
    public List<Guid> PermissionDefinitionIds { get; init; } = [];
}

/// <summary>
/// Input for querying permission groups with optional filters and pagination.
/// </summary>
public sealed class GetPermissionGroupsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Optional text filter to search by group name or display name.
    /// </summary>
    public string? Filter { get; init; }

    /// <summary>
    /// Optional filter to restrict results to a specific owning microservice.
    /// </summary>
    public string? ServiceName { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionTemplate Inputs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Input for creating a new permission template.
/// </summary>
public sealed class CreatePermissionTemplateInput
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
    /// Collection of permission group IDs to include in this template.
    /// </summary>
    public List<Guid> GroupIds { get; init; } = [];
}

/// <summary>
/// Input for updating an existing permission template.
/// </summary>
public sealed class UpdatePermissionTemplateInput
{
    /// <summary>
    /// Updated human-readable display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional updated template description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated collection of permission group IDs to include in this template.
    /// </summary>
    public List<Guid> GroupIds { get; init; } = [];
}

/// <summary>
/// Input for querying permission templates with optional filter and pagination.
/// </summary>
public sealed class GetPermissionTemplatesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Optional text filter to search by template name or display name.
    /// </summary>
    public string? Filter { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGrant Inputs
// ──────────────────────────────────────────────────────────────────

/// <summary>
/// Input for granting a permission to a provider (role or user).
/// </summary>
public sealed class GrantPermissionInput
{
    /// <summary>
    /// Unique permission name to grant.
    /// </summary>
    public required string PermissionName { get; init; }

    /// <summary>
    /// Provider type that receives the grant (e.g., <c>R</c> for Role, <c>U</c> for User).
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Provider-specific key (e.g., role ID or user ID).
    /// </summary>
    public required string ProviderKey { get; init; }
}

/// <summary>
/// Input for revoking a permission from a provider (role or user).
/// </summary>
public sealed class RevokePermissionInput
{
    /// <summary>
    /// Unique permission name to revoke.
    /// </summary>
    public required string PermissionName { get; init; }

    /// <summary>
    /// Provider type that currently holds the grant (e.g., <c>R</c> for Role, <c>U</c> for User).
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Provider-specific key (e.g., role ID or user ID).
    /// </summary>
    public required string ProviderKey { get; init; }
}

/// <summary>
/// Input for querying permission grants for a specific provider.
/// </summary>
public sealed class GetPermissionGrantsInput
{
    /// <summary>
    /// Provider type to query grants for (e.g., <c>R</c> for Role, <c>U</c> for User).
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Provider-specific key to query grants for (e.g., role ID or user ID).
    /// </summary>
    public required string ProviderKey { get; init; }
}
