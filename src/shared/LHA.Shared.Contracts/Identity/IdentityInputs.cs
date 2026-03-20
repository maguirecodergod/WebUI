using LHA.Core;
using LHA.Ddd.Application;
using LHA.Identity.Domain.Shared;

namespace LHA.Shared.Contracts.Identity;

// ─── User Inputs ─────────────────────────────────────────────────────

/// <summary>Input for creating a new user.</summary>
public sealed class CreateIdentityUserInput
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public bool LockoutEnabled { get; init; } = true;
    public List<Guid> RoleIds { get; init; } = [];
}

/// <summary>Input for updating an existing user.</summary>
public sealed class UpdateIdentityUserInput
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
    public bool LockoutEnabled { get; init; } = true;
    public List<Guid> RoleIds { get; init; } = [];
    public required string ConcurrencyStamp { get; init; }
}

/// <summary>Input for changing a user's password.</summary>
public sealed class ChangePasswordInput
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}

/// <summary>Input for querying users with filtering and paging.</summary>
public sealed class GetIdentityUsersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public CMasterStatus? Status { get; init; }
    public Guid? RoleId { get; init; }
}

// ─── Role Inputs ─────────────────────────────────────────────────────

/// <summary>Input for creating a new role.</summary>
public sealed class CreateIdentityRoleInput
{
    public required string Name { get; init; }
    public bool IsDefault { get; init; }
    public bool IsPublic { get; init; }
}

/// <summary>Input for updating an existing role.</summary>
public sealed class UpdateIdentityRoleInput
{
    public required string Name { get; init; }
    public bool IsDefault { get; init; }
    public bool IsPublic { get; init; }
    public required string ConcurrencyStamp { get; init; }
}

/// <summary>Input for querying roles.</summary>
public sealed class GetIdentityRolesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public CMasterStatus? Status { get; init; }
}

// ─── Claim Type Inputs ───────────────────────────────────────────────

/// <summary>Input for creating or updating a claim type.</summary>
public sealed class CreateOrUpdateClaimTypeInput
{
    public required string Name { get; init; }
    public bool Required { get; init; }
    public CIdentityClaimValueType ValueType { get; init; } = CIdentityClaimValueType.String;
    public string? Regex { get; init; }
    public string? RegexDescription { get; init; }
    public string? Description { get; init; }
}

/// <summary>Input for querying claim types.</summary>
public sealed class GetClaimTypesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
}

// ─── Security Log Inputs ─────────────────────────────────────────────

/// <summary>Input for querying security logs.</summary>
public sealed class GetSecurityLogsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; init; }
    public Guid? UserId { get; init; }
    public string? Action { get; init; }
}

// ─── Auth Inputs ─────────────────────────────────────────────────────

/// <summary>Input for login.</summary>
public sealed class LoginInput
{
    public required string UserNameOrEmail { get; init; }
    public required string Password { get; init; }
}

/// <summary>Input for refreshing tokens.</summary>
public sealed class RefreshTokenInput
{
    public required string RefreshToken { get; init; }
}

// ─── Permission Inputs ───────────────────────────────────────────────

/// <summary>Input for querying permissions for a provider.</summary>
public sealed class GetPermissionListInput
{
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
}

/// <summary>Input for granting or revoking a set of permissions.</summary>
public sealed class UpdatePermissionsInput
{
    public required string ProviderName { get; init; }
    public required string ProviderKey { get; init; }
    public List<PermissionGrantInput> Permissions { get; init; } = [];
}

/// <summary>A single permission grant/revoke entry.</summary>
public sealed class PermissionGrantInput
{
    public required string Name { get; init; }
    public bool IsGranted { get; init; }
}
