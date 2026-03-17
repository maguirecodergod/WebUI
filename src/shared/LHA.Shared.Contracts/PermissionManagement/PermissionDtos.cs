using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.PermissionManagement;

// ──────────────────────────────────────────────────────────────────
//  PermissionDefinition DTOs
// ──────────────────────────────────────────────────────────────────

public sealed class PermissionDefinitionDto : EntityDto<Guid>
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string ServiceName { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGroup DTOs
// ──────────────────────────────────────────────────────────────────

public sealed class PermissionGroupDto : FullAuditedEntityDto<Guid>
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string ServiceName { get; init; }
    public string? Description { get; init; }
    public List<PermissionDefinitionDto> Permissions { get; init; } = [];
}

// ──────────────────────────────────────────────────────────────────
//  PermissionTemplate DTOs
// ──────────────────────────────────────────────────────────────────

public sealed class PermissionTemplateDto : FullAuditedEntityDto<Guid>
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public List<PermissionGroupDto> Groups { get; init; } = [];
}

// ──────────────────────────────────────────────────────────────────
//  PermissionGrant DTOs
// ──────────────────────────────────────────────────────────────────

public sealed class PermissionGrantDto : EntityDto<Guid>
{
    public Guid? TenantId { get; init; }
    public required string Name { get; init; }
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
}
