using LHA.Ddd.Application;

namespace LHA.PermissionManagement.Application.Contracts;

// ──────────────────────────────────────────────────────────────────
//  PermissionDefinition Inputs
// ──────────────────────────────────────────────────────────────────

public sealed class CreatePermissionDefinitionInput
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string ServiceName { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
}

public sealed class GetPermissionDefinitionsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public string? ServiceName { get; init; }
    public string? GroupName { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGroup Inputs
// ──────────────────────────────────────────────────────────────────

public sealed class CreatePermissionGroupInput
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string ServiceName { get; init; }
    public string? Description { get; init; }
    public List<Guid> PermissionDefinitionIds { get; init; } = [];
}

public sealed class UpdatePermissionGroupInput
{
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public List<Guid> PermissionDefinitionIds { get; init; } = [];
}

public sealed class GetPermissionGroupsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public string? ServiceName { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionTemplate Inputs
// ──────────────────────────────────────────────────────────────────

public sealed class CreatePermissionTemplateInput
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public List<Guid> GroupIds { get; init; } = [];
}

public sealed class UpdatePermissionTemplateInput
{
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public List<Guid> GroupIds { get; init; } = [];
}

public sealed class GetPermissionTemplatesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGrant Inputs
// ──────────────────────────────────────────────────────────────────

public sealed class GrantPermissionInput
{
    public required string PermissionName { get; init; }
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
}

public sealed class RevokePermissionInput
{
    public required string PermissionName { get; init; }
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
}

public sealed class GetPermissionGrantsInput
{
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
}
