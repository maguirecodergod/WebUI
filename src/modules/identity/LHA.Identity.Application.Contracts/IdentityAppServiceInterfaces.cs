using LHA.Ddd.Application;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Users;
using LHA.Shared.Contracts.Identity.Roles;
using LHA.Shared.Contracts.Identity.Auth;
using LHA.Shared.Contracts.Identity.Claims;
using LHA.Shared.Contracts.Identity.Audit;
using LHA.Shared.Contracts.Identity.Permissions;

namespace LHA.Identity.Application.Contracts;

/// <summary>
/// Application service for user management.
/// </summary>
public interface IIdentityUserAppService
    : ICrudAppService<IdentityUserDto, Guid, GetIdentityUsersInput, CreateIdentityUserInput, UpdateIdentityUserInput>
{
    /// <summary>Finds a user by user name.</summary>
    Task<IdentityUserDto?> FindByUserNameAsync(string userName, CancellationToken ct = default);

    /// <summary>Finds a user by email.</summary>
    Task<IdentityUserDto?> FindByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>Gets the list of roles assigned to a user.</summary>
    Task<List<IdentityRoleDto>> GetRolesAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Updates the roles assigned to a user.</summary>
    Task<IdentityUserDto> UpdateRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken ct = default);

    /// <summary>Changes a user's password.</summary>
    Task ChangePasswordAsync(Guid userId, ChangePasswordInput input, CancellationToken ct = default);

    /// <summary>Activates a user.</summary>
    Task<IdentityUserDto> ActivateAsync(Guid id, CancellationToken ct = default);

    /// <summary>Deactivates a user.</summary>
    Task<IdentityUserDto> DeactivateAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Application service for role management.
/// </summary>
public interface IIdentityRoleAppService
    : ICrudAppService<IdentityRoleDto, Guid, GetIdentityRolesInput, CreateIdentityRoleInput, UpdateIdentityRoleInput>
{
    /// <summary>Returns all roles (no paging).</summary>
    Task<List<IdentityRoleDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Activates a role.</summary>
    Task<IdentityRoleDto> ActivateAsync(Guid id, CancellationToken ct = default);

    /// <summary>Deactivates a role.</summary>
    Task<IdentityRoleDto> DeactivateAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Application service for authentication (login, register, refresh, current user).
/// </summary>
public interface IAuthAppService : IApplicationService
{
    /// <summary>Authenticates a user and returns JWT tokens.</summary>
    Task<AuthResultDto> LoginAsync(LoginInput input, CancellationToken ct = default);

    /// <summary>Registers a new user account.</summary>
    Task<IdentityUserDto> RegisterAsync(CreateIdentityUserInput input, CancellationToken ct = default);

    /// <summary>Registers a new tenant and its initial admin user.</summary>
    Task<AuthResultDto> RegisterTenantAsync(RegisterTenantInput input, CancellationToken ct = default);

    /// <summary>Refreshes an access token using a refresh token.</summary>
    Task<AuthResultDto> RefreshTokenAsync(RefreshTokenInput input, CancellationToken ct = default);

    /// <summary>Gets the current authenticated user's profile.</summary>
    Task<CurrentUserDto> GetCurrentUserAsync(Guid userId, CancellationToken ct = default);
}

/// <summary>
/// Application service for claim type management.
/// </summary>
public interface IIdentityClaimTypeAppService : IApplicationService
{
    Task<IdentityClaimTypeDto> GetAsync(Guid id, CancellationToken ct = default);
    Task<PagedResultDto<IdentityClaimTypeDto>> GetListAsync(GetClaimTypesInput input, CancellationToken ct = default);
    Task<IdentityClaimTypeDto> CreateAsync(CreateOrUpdateClaimTypeInput input, CancellationToken ct = default);
    Task<IdentityClaimTypeDto> UpdateAsync(Guid id, CreateOrUpdateClaimTypeInput input, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Application service for security log queries.
/// </summary>
public interface IIdentitySecurityLogAppService : IApplicationService
{
    Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetSecurityLogsInput input, CancellationToken ct = default);
}

/// <summary>
/// Application service for permission grant management.
/// </summary>
public interface IPermissionAppService : IApplicationService
{
    /// <summary>Gets all permissions for a provider (role or user) with grant status.</summary>
    Task<List<PermissionGrantDto>> GetAsync(GetPermissionListInput input, CancellationToken ct = default);

    /// <summary>Batch update permission grants for a provider.</summary>
    Task UpdateAsync(UpdatePermissionsInput input, CancellationToken ct = default);
}
