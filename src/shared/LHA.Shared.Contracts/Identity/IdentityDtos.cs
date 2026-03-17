using LHA.Core;
using LHA.Ddd.Application;
using LHA.Identity.Domain.Shared;

namespace LHA.Shared.Contracts.Identity;

/// <summary>DTO for <c>IdentityUser</c>.</summary>
public sealed class IdentityUserDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public bool EmailConfirmed { get; init; }
    public string? PhoneNumber { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public bool LockoutEnabled { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public int AccessFailedCount { get; init; }
    public CMasterStatus Status { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public string ConcurrencyStamp { get; init; } = string.Empty;
    public List<IdentityUserRoleDto> Roles { get; init; } = [];
}

/// <summary>DTO for a user-role assignment.</summary>
public sealed class IdentityUserRoleDto
{
    public Guid RoleId { get; init; }
    public string? RoleName { get; init; }
}

/// <summary>DTO for <c>IdentityRole</c>.</summary>
public sealed class IdentityRoleDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; init; }
    public required string Name { get; init; }
    public CMasterStatus Status { get; init; }
    public bool IsDefault { get; init; }
    public bool IsStatic { get; init; }
    public bool IsPublic { get; init; }
    public string ConcurrencyStamp { get; init; } = string.Empty;
}

/// <summary>DTO for <c>IdentityClaimType</c>.</summary>
public sealed class IdentityClaimTypeDto : FullAuditedEntityDto<Guid>
{
    public required string Name { get; init; }
    public bool Required { get; init; }
    public bool IsStatic { get; init; }
    public IdentityClaimValueType ValueType { get; init; }
    public string? Regex { get; init; }
    public string? RegexDescription { get; init; }
    public string? Description { get; init; }
}

/// <summary>DTO for <c>IdentitySecurityLog</c>.</summary>
public sealed class IdentitySecurityLogDto : CreationAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; init; }
    public string? ApplicationName { get; init; }
    public string? Identity { get; init; }
    public required string Action { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public string? TenantName { get; init; }
    public string? ClientId { get; init; }
    public string? CorrelationId { get; init; }
    public string? ClientIpAddress { get; init; }
    public string? BrowserInfo { get; init; }
}

/// <summary>DTO for a permission grant.</summary>
public sealed class PermissionGrantDto
{
    public required string Name { get; init; }
    public bool IsGranted { get; init; }
}

/// <summary>DTO for permission grants organized by provider.</summary>
public sealed class PermissionWithGrantInfoDto
{
    public required string Name { get; init; }
    public List<ProviderGrantInfoDto> Providers { get; init; } = [];
}

/// <summary>Provider-level grant information.</summary>
public sealed class ProviderGrantInfoDto
{
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
    public bool IsGranted { get; init; }
}

/// <summary>Auth token pair returned after login / refresh.</summary>
public sealed class AuthResultDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public long ExpiresIn { get; init; }
}

/// <summary>DTO for the currently authenticated user.</summary>
public sealed class CurrentUserDto
{
    public Guid Id { get; init; }
    public Guid? TenantId { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public List<string> Roles { get; init; } = [];
}
