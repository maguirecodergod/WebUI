using LHA.Core;
using LHA.Ddd.Application;
using LHA.Shared.Domain.Identity;
// using LHA.Identity.Domain.Shared;

namespace LHA.Shared.Contracts.Identity;

/// <summary>DTO for <c>IdentityUser</c>.</summary>
public sealed class IdentityUserDto : FullAuditedEntityDto<Guid>
{
    /// <summary>Tenant the user belongs to, or <c>null</c> for a host-side user.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>Unique login name of the user.</summary>
    public required string UserName { get; set; }

    /// <summary>Primary email address of the user.</summary>
    public required string Email { get; set; }

    /// <summary>Indicates whether the user's email address has been verified.</summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>Phone number associated with the user, if provided.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Indicates whether the user's phone number has been verified.</summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>Indicates whether two-factor authentication is enabled for the user.</summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>Indicates whether the account is subject to lockout after repeated failed login attempts.</summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>UTC timestamp when the user's lockout expires, or <c>null</c> if not locked out.</summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>Number of consecutive failed login attempts since the last successful sign-in.</summary>
    public int AccessFailedCount { get; set; }

    /// <summary>Lifecycle status of the user account. See <see cref="CMasterStatus"/>.</summary>
    public CMasterStatus Status { get; set; }

    /// <summary>Given (first) name of the user.</summary>
    public string? Name { get; set; }

    /// <summary>Family (last) name of the user.</summary>
    public string? Surname { get; set; }

    /// <summary>Concurrency token used for optimistic locking during updates.</summary>
    public string ConcurrencyStamp { get; set; } = string.Empty;

    /// <summary>Collection of roles currently assigned to the user.</summary>
    public List<IdentityUserRoleDto> Roles { get; set; } = [];
}

/// <summary>DTO for a user-role assignment.</summary>
public sealed class IdentityUserRoleDto
{
    /// <summary>Unique identifier of the assigned role.</summary>
    public Guid RoleId { get; init; }

    /// <summary>Display name of the assigned role.</summary>
    public string? RoleName { get; init; }
}

/// <summary>DTO for <c>IdentityRole</c>.</summary>
public sealed class IdentityRoleDto : FullAuditedEntityDto<Guid>
{
    /// <summary>Tenant the role belongs to, or <c>null</c> for a host-level role.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>Display name of the role.</summary>
    public required string Name { get; init; }

    /// <summary>Lifecycle status of the role. See <see cref="CMasterStatus"/>.</summary>
    public CMasterStatus Status { get; init; }

    /// <summary>Indicates whether the role is assigned to new users by default upon registration.</summary>
    public bool IsDefault { get; init; }

    /// <summary>Indicates whether the role is system-managed and cannot be renamed or deleted.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Indicates whether the role is visible to tenant administrators in the UI.</summary>
    public bool IsPublic { get; init; }

    /// <summary>Concurrency token used for optimistic locking during updates.</summary>
    public string ConcurrencyStamp { get; init; } = string.Empty;
}

/// <summary>DTO for <c>IdentityClaimType</c>.</summary>
public sealed class IdentityClaimTypeDto : FullAuditedEntityDto<Guid>
{
    /// <summary>Unique name of the claim type.</summary>
    public required string Name { get; init; }

    /// <summary>Indicates whether the claim must be provided by every user.</summary>
    public bool Required { get; init; }

    /// <summary>Indicates whether the claim type is system-managed and cannot be deleted.</summary>
    public bool IsStatic { get; init; }

    /// <summary>Expected data type of the claim value. See <see cref="CIdentityClaimValueType"/>.</summary>
    public CIdentityClaimValueType ValueType { get; init; }

    /// <summary>Regular expression used to validate the claim value, if applicable.</summary>
    public string? Regex { get; init; }

    /// <summary>Human-readable description of the validation regex pattern.</summary>
    public string? RegexDescription { get; init; }

    /// <summary>Descriptive text explaining the purpose of the claim type.</summary>
    public string? Description { get; init; }
}

/// <summary>DTO for <c>IdentitySecurityLog</c>.</summary>
public sealed class IdentitySecurityLogDto : CreationAuditedEntityDto<Guid>
{
    /// <summary>Tenant in which the security event occurred, or <c>null</c> for host-level events.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>Name of the application that generated the security event.</summary>
    public string? ApplicationName { get; init; }

    /// <summary>Identity provider used during the event (e.g. <c>"local"</c>, <c>"OpenId"</c>).</summary>
    public string? Identity { get; init; }

    /// <summary>Action code describing the security event (e.g. <c>"Login"</c>, <c>"Logout"</c>, <c>"ChangePassword"</c>).</summary>
    public required string Action { get; init; }

    /// <summary>Unique identifier of the user associated with the event.</summary>
    public Guid? UserId { get; init; }

    /// <summary>Login name of the user associated with the event.</summary>
    public string? UserName { get; init; }

    /// <summary>Name of the tenant in which the event occurred.</summary>
    public string? TenantName { get; init; }

    /// <summary>Client identifier from which the request originated (e.g. an OAuth client id).</summary>
    public string? ClientId { get; init; }

    /// <summary>Correlation identifier used to trace the request across services.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>IP address of the client that triggered the event.</summary>
    public string? ClientIpAddress { get; init; }

    /// <summary>User-agent / browser information from the client request.</summary>
    public string? BrowserInfo { get; init; }
}

/// <summary>DTO for a permission grant.</summary>
public sealed class PermissionGrantDto
{
    /// <summary>Unique name of the permission (e.g. <c>"Identity.Users.Create"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>Indicates whether the permission is currently granted.</summary>
    public bool IsGranted { get; set; }
}

/// <summary>DTO for permission grants organized by provider.</summary>
public sealed class PermissionWithGrantInfoDto
{
    /// <summary>Unique name of the permission (e.g. <c>"Identity.Users.Create"</c>).</summary>
    public required string Name { get; init; }

    /// <summary>Collection of provider-level grant details for this permission.</summary>
    public List<ProviderGrantInfoDto> Providers { get; init; } = [];
}

/// <summary>Provider-level grant information.</summary>
public sealed class ProviderGrantInfoDto
{
    /// <summary>Category of the provider (e.g. <c>"R"</c> for role, <c>"U"</c> for user).</summary>
    public required string ProviderName { get; init; }

    /// <summary>Identifier of the specific provider instance (e.g. the role or user id).</summary>
    public required string ProviderKey { get; init; }

    /// <summary>Indicates whether the permission is granted by this provider.</summary>
    public bool IsGranted { get; init; }
}

/// <summary>Auth token pair returned after login / refresh.</summary>
public sealed class AuthResultDto
{
    /// <summary>JWT access token used to authorize subsequent API requests.</summary>
    public string? AccessToken { get; init; }

    /// <summary>Opaque token used to obtain a new access token after expiry.</summary>
    public string? RefreshToken { get; init; }

    /// <summary>Lifetime of the access token in seconds.</summary>
    public long ExpiresIn { get; init; }

    /// <summary>Indicates that the user belongs to multiple tenants and must select one before proceeding.</summary>
    public bool RequiresTenantSelection { get; init; }

    /// <summary>Tenants accessible to the authenticated user, populated when <see cref="RequiresTenantSelection"/> is <c>true</c>.</summary>
    public List<UserTenantDto>? Tenants { get; init; }
}

/// <summary>DTO for a tenant linked to a user.</summary>
public sealed class UserTenantDto
{
    /// <summary>Unique identifier of the tenant.</summary>
    public Guid Id { get; init; }

    /// <summary>Display name of the tenant.</summary>
    public required string Name { get; init; }
}

/// <summary>DTO for the currently authenticated user.</summary>
public sealed class CurrentUserDto
{
    /// <summary>Unique identifier of the authenticated user.</summary>
    public Guid Id { get; init; }

    /// <summary>Tenant the user belongs to, or <c>null</c> for a host-side user.</summary>
    public Guid? TenantId { get; init; }

    /// <summary>Unique login name of the authenticated user.</summary>
    public required string UserName { get; init; }

    /// <summary>Primary email address of the authenticated user.</summary>
    public required string Email { get; init; }

    /// <summary>Given (first) name of the authenticated user.</summary>
    public string? Name { get; init; }

    /// <summary>Family (last) name of the authenticated user.</summary>
    public string? Surname { get; init; }

    /// <summary>Names of roles currently assigned to the authenticated user.</summary>
    public List<string> Roles { get; init; } = [];

    /// <summary>Names of permissions currently granted to the authenticated user.</summary>
    public List<string> Permissions { get; init; } = [];
}
