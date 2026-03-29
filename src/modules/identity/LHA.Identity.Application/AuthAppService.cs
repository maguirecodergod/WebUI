using LHA.Auditing;
using LHA.Core;
using LHA.Ddd.Application;
using LHA.EventBus;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;
using LHA.UnitOfWork;

namespace LHA.Identity.Application;

/// <summary>
/// Application service for authentication: login, register, refresh, current user.
/// </summary>
public sealed class AuthAppService : ApplicationService, IAuthAppService
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IPermissionGrantRepository _permissionGrantRepository;
    private readonly IdentityUserManager _userManager;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IUnitOfWorkManager _uowManager;
    private readonly IEventBus _eventBus;
    private readonly IIdentitySecurityLogRepository _securityLogRepository;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly IUserTenantLookupService _userTenantLookupService;
    private readonly IPermissionStore _permissionStore;
    private readonly ITenantManagerBridge _tenantManagerBridge;
    private readonly ICurrentTenant _currentTenant;

    public const string SystemSuperAdminRole = "SystemSuperAdmin";
    public const string TenantAdminRole = "TenantAdmin";

    public AuthAppService(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IPermissionGrantRepository permissionGrantRepository,
        IdentityUserManager userManager,
        ILookupNormalizer lookupNormalizer,
        JwtTokenService jwtTokenService,
        IUnitOfWorkManager uowManager,
        IEventBus eventBus,
        IIdentitySecurityLogRepository securityLogRepository,
        IClientInfoProvider clientInfoProvider,
        IUserTenantLookupService userTenantLookupService,
        IPermissionStore permissionStore,
        ITenantManagerBridge tenantManagerBridge,
        ICurrentTenant currentTenant)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionGrantRepository = permissionGrantRepository;
        _userManager = userManager;
        _lookupNormalizer = lookupNormalizer;
        _jwtTokenService = jwtTokenService;
        _uowManager = uowManager;
        _eventBus = eventBus;
        _securityLogRepository = securityLogRepository;
        _clientInfoProvider = clientInfoProvider;
        _userTenantLookupService = userTenantLookupService;
        _permissionStore = permissionStore;
        _tenantManagerBridge = tenantManagerBridge;
        _currentTenant = currentTenant;
    }

    /// <inheritdoc />
    public async Task<AuthResultDto> LoginAsync(LoginInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        // Find user by email or username
        var user = await FindUserByLoginAsync(input.UserNameOrEmail, ct);

        if (user is null)
        {
            await RecordSecurityLogAsync("Login", IdentitySecurityLogActionConsts.LoginFailed,
                null, input.UserNameOrEmail, ct);

            await _eventBus.PublishAsync(new LoginFailedEto(
                input.UserNameOrEmail, "InvalidUserNameOrEmail", null, DateTimeOffset.UtcNow), ct);

            throw new UnauthorizedAccessException("Invalid user name or email.");
        }

        // Check status
        if (user.Status != CMasterStatus.Active)
        {
            await RecordSecurityLogAsync("Login", IdentitySecurityLogActionConsts.LoginNotAllowed,
                user.Id, user.UserName, ct);

            throw new UnauthorizedAccessException("User account is not active.");
        }

        // Check lockout
        if (user.IsLockedOut)
        {
            await RecordSecurityLogAsync("Login", IdentitySecurityLogActionConsts.LoginLockedout,
                user.Id, user.UserName, ct);

            throw new UnauthorizedAccessException(
                $"User account is locked until {user.LockoutEnd:O}.");
        }

        // Verify password
        if (!_userManager.VerifyPassword(user, input.Password))
        {
            user.IncrementAccessFailedCount();
            await _userRepository.UpdateAsync(user);
            await uow.CompleteAsync();

            await _eventBus.PublishAsync(new LoginFailedEto(
                input.UserNameOrEmail, "InvalidPassword", user.TenantId, DateTimeOffset.UtcNow), ct);

            throw new UnauthorizedAccessException("Invalid password.");
        }

        // Success — reset failed count
        user.ResetAccessFailedCount();

        // 1. Determine target tenant from current context (resolved by ICurrentTenant)
        // If _currentTenant.Id is null, the request is at the Host level.
        var targetTenantId = _currentTenant.Id;

        // 2. Fetch roles for the target tenant
        var (roleIds, roleNames) = await GetRolesAsync(user, targetTenantId, ct);

        // 3. System Super Admin: login directly with all permissions (Host only)
        if (roleNames.Contains(SystemSuperAdminRole) && user.TenantId == null)
        {
            var allPermissions = await _permissionStore.GetAllPermissionsAsync(ct);
            var accessToken = _jwtTokenService.GenerateAccessToken(
                user.Id, user.UserName, user.Email, user.TenantId, roleNames, allPermissions);
            var refreshToken = JwtTokenService.GenerateRefreshToken();
            var refreshExpiry = _jwtTokenService.GetRefreshTokenExpiration();

            user.SetToken(JwtTokenService.RefreshTokenProvider, JwtTokenService.RefreshTokenName,
                refreshToken, refreshExpiry);

            await _userRepository.UpdateAsync(user);
            await RecordSecurityLogAsync("Login", IdentitySecurityLogActionConsts.LoginSucceeded,
                user.Id, user.UserName, ct);
            await _eventBus.PublishAsync(new LoginSucceededEto(
                user.Id, user.UserName, user.TenantId, DateTimeOffset.UtcNow), ct);

            await uow.CompleteAsync();

            return new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtTokenService.AccessTokenExpiresInSeconds,
            };
        }

        // 4. Tenant Admin & Tenant Selection logic
        if (_currentTenant.Id == null && roleNames.Contains(TenantAdminRole))
        {
            var userTenants = await GetUserTenantsAsync(user, ct);
            // If user belongs to multiple tenants and hasn't selected one yet via context (header/host), return the list
            if (userTenants.Count > 1)
            {
                await uow.CompleteAsync();
                return new AuthResultDto
                {
                    RequiresTenantSelection = true,
                    Tenants = userTenants
                };
            }
            
            // If user only has one tenant, we should probably redirect/re-login for that tenant if context is Host
            if (userTenants.Count == 1 && userTenants[0].Id != targetTenantId)
            {
                targetTenantId = userTenants[0].Id;
                (roleIds, roleNames) = await GetRolesAsync(user, targetTenantId, ct);
            }
        }
        else if (_currentTenant.Id != null)
        {
            // Validate user actually has access to the current tenant resolved from context
            if (!user.Roles.Any(r => r.TenantId == _currentTenant.Id))
            {
                throw new UnauthorizedAccessException($"User does not have access to the current tenant '{_currentTenant.Id}'.");
            }
        }

        // 5. Finalize login for the selected/default tenant
        var permissions = await ResolvePermissionsAsync(user.Id, roleIds, targetTenantId, ct);
        var accessTokenFinal = _jwtTokenService.GenerateAccessToken(
            user.Id, user.UserName, user.Email, targetTenantId, roleNames, permissions);
        var refreshTokenFinal = JwtTokenService.GenerateRefreshToken();
        var refreshExpiryFinal = _jwtTokenService.GetRefreshTokenExpiration();

        user.SetToken(JwtTokenService.RefreshTokenProvider, JwtTokenService.RefreshTokenName,
            refreshTokenFinal, refreshExpiryFinal);

        await _userRepository.UpdateAsync(user);

        await RecordSecurityLogAsync("Login", IdentitySecurityLogActionConsts.LoginSucceeded,
            user.Id, user.UserName, ct);
        await _eventBus.PublishAsync(new LoginSucceededEto(
            user.Id, user.UserName, targetTenantId, DateTimeOffset.UtcNow), ct);

        await uow.CompleteAsync();

        return new AuthResultDto
        {
            AccessToken = accessTokenFinal,
            RefreshToken = refreshTokenFinal,
            ExpiresIn = _jwtTokenService.AccessTokenExpiresInSeconds,
        };
    }

    /// <inheritdoc />
    public async Task<AuthResultDto> RegisterTenantAsync(RegisterTenantInput input, CancellationToken ct = default)
    {
        // 1. Create Tenant (via Bridge)
        var tenantId = await _tenantManagerBridge.CreateTenantAsync(input.TenantName, input.DatabaseStyle, ct);

        // 2. Change ambient context to the new tenant so EF uses its connection settings
        using (_currentTenant.Change(tenantId))
        {
            await using var uow = _uowManager.Begin(new UnitOfWorkOptions { IsTransactional = true });

            var user = await _userManager.CreateAsync(
                input.AdminUserName,
                input.AdminEmail,
                input.AdminPassword,
                tenantId);

            // 3. Assign TenantAdmin role to the new user
            var normalizedRoleName = _lookupNormalizer.NormalizeName(TenantAdminRole);
            var role = await _roleRepository.FindByNormalizedNameAsync(normalizedRoleName, ct);
            if (role != null)
            {
                user.AddRole(role.Id);
            }

            await _userRepository.InsertAsync(user, ct);

            // Commit UoW before generating tokens to ensure data is persistent
            await uow.CompleteAsync();
        }

        // 4. Automated login for the new admin
        // We reuse LoginAsync logic by passing the credentials or just generating token directly
        return await LoginAsync(new LoginInput
        {
            UserNameOrEmail = input.AdminUserName,
            Password = input.AdminPassword
        }, ct);
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto> RegisterAsync(CreateIdentityUserInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userManager.CreateAsync(
            input.UserName, input.Email, input.Password, cancellationToken: ct);

        if (input.PhoneNumber is not null) user.SetPhoneNumber(input.PhoneNumber);
        if (input.Name is not null) user.SetName(input.Name);
        if (input.Surname is not null) user.SetSurname(input.Surname);

        await _userRepository.InsertAsync(user);

        await RecordSecurityLogAsync("Register", IdentitySecurityLogActionConsts.LoginSucceeded,
            user.Id, user.UserName, ct);
        await _eventBus.PublishAsync(new UserCreatedEto(
            user.Id, user.UserName, user.Email, user.TenantId, user.CreationTime), ct);

        await uow.CompleteAsync();

        return MapToDto(user);
    }

    /// <inheritdoc />
    public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenInput input, CancellationToken ct)
    {
        await using var uow = _uowManager.Begin(
            new UnitOfWorkOptions { IsTransactional = true });

        var user = await _userRepository.FindByTokenAsync(
            JwtTokenService.RefreshTokenProvider,
            JwtTokenService.RefreshTokenName,
            input.RefreshToken, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Check status
        if (user.Status != CMasterStatus.Active)
            throw new UnauthorizedAccessException("User account is not active.");

        // Rotate tokens
        var (roleIds, roleNames) = await GetRolesAsync(user, user.TenantId, ct);
        var permissions = await ResolvePermissionsAsync(user.Id, roleIds, user.TenantId, ct);
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id, user.UserName, user.Email, user.TenantId, roleNames, permissions);
        var refreshToken = JwtTokenService.GenerateRefreshToken();
        var refreshExpiry = _jwtTokenService.GetRefreshTokenExpiration();

        user.SetToken(JwtTokenService.RefreshTokenProvider, JwtTokenService.RefreshTokenName,
            refreshToken, refreshExpiry);

        await _userRepository.UpdateAsync(user);
        await uow.CompleteAsync();

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtTokenService.AccessTokenExpiresInSeconds,
        };
    }

    /// <inheritdoc />
    public async Task<CurrentUserDto> GetCurrentUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetAsync(userId, ct);
        var (roleIds, roleNames) = await GetRolesAsync(user, user.TenantId, ct);

        List<string> permissions;
        if (roleNames.Contains(SystemSuperAdminRole) && user.TenantId == null)
        {
            permissions = await _permissionStore.GetAllPermissionsAsync(ct);
        }
        else
        {
            permissions = await ResolvePermissionsAsync(user.Id, roleIds, user.TenantId, ct);
        }

        return new CurrentUserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Roles = [.. roleNames],
            Permissions = permissions
        };
    }

    // ─── Helpers ─────────────────────────────────────────────────────

    private async Task<List<UserTenantDto>> GetUserTenantsAsync(IdentityUser user, CancellationToken ct)
    {
        var tenantIds = user.Roles
            .Where(r => r.TenantId != null)
            .Select(r => r.TenantId!.Value)
            .Distinct()
            .ToList();

        if (tenantIds.Count == 0) return [];

        var tenants = await _userTenantLookupService.GetTenantsAsync(tenantIds, ct);
        return tenants.ConvertAll(t => new UserTenantDto
        {
            Id = t.Id,
            Name = t.Name
        });
    }

    private async Task<IdentityUser?> FindUserByLoginAsync(string userNameOrEmail, CancellationToken ct)
    {
        var normalized = _lookupNormalizer.NormalizeName(userNameOrEmail);

        // Try email first if it contains @
        if (userNameOrEmail.Contains('@'))
        {
            var byEmail = await _userRepository.FindByNormalizedEmailAsync(normalized, ct);
            if (byEmail is not null) return byEmail;
        }

        return await _userRepository.FindByNormalizedUserNameAsync(normalized, ct);
    }

    private async Task<(List<Guid> Ids, List<string> Names)> GetRolesAsync(
        IdentityUser user, Guid? tenantId, CancellationToken ct)
    {
        // Filter roles by specifically requested tenant, or user's base tenant
        var roleIds = user.Roles
            .Where(r => r.TenantId == tenantId)
            .Select(r => r.RoleId)
            .ToList();

        if (roleIds.Count == 0) return ([], []);

        var roles = await _roleRepository.GetListByIdsAsync(roleIds, ct);
        return (roleIds, roles.ConvertAll(r => r.Name));
    }

    /// <summary>
    /// Resolves all permissions for a user by querying role-based ("R") and
    /// direct user-based ("U") permission grants.
    /// <para>Priority: user deny > user allow > role deny > role allow.</para>
    /// </summary>
    private async Task<List<string>> ResolvePermissionsAsync(
        Guid userId, List<Guid> roleIds, Guid? tenantId, CancellationToken ct)
    {
        // 1. Role-based grants (ProviderName="R", ProviderKey=roleId)
        var roleResult = new Dictionary<string, bool>(StringComparer.Ordinal);
        if (roleIds.Count > 0)
        {
            var roleKeys = roleIds.ConvertAll(id => id.ToString());
            var roleGrants = await _permissionGrantRepository
                .GetGrantsByProvidersAsync("R", roleKeys, ct);

            foreach (var g in roleGrants)
            {
                // Among roles: deny wins (if any role denies, the permission is denied)
                if (roleResult.TryGetValue(g.Name, out var existing))
                {
                    if (!g.IsGranted) roleResult[g.Name] = false;
                }
                else
                {
                    roleResult[g.Name] = g.IsGranted;
                }
            }
        }

        // 2. User-specific grants (ProviderName="U", ProviderKey=userId)
        var userGrants = await _permissionGrantRepository
            .GetGrantsByProvidersAsync("U", [userId.ToString()], ct);

        // 3. Merge: user overrides role
        var final = new Dictionary<string, bool>(roleResult, StringComparer.Ordinal);
        foreach (var g in userGrants)
            final[g.Name] = g.IsGranted;

        return final.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
    }

    private async Task RecordSecurityLogAsync(
        string identity, string action, Guid? userId, string? userName, CancellationToken ct)
    {
        var log = new IdentitySecurityLog(
            id: Guid.CreateVersion7(),
            action: action,
            identity: identity,
            userId: userId,
            userName: userName,
            applicationName: "LHA.Identity",
            correlationId: _clientInfoProvider.CorrelationId,
            clientIpAddress: _clientInfoProvider.ClientIpAddress,
            browserInfo: _clientInfoProvider.BrowserInfo);

        await _securityLogRepository.InsertAsync(log, ct);
    }

    private static IdentityUserDto MapToDto(IdentityUser user) => new()
    {
        Id = user.Id,
        TenantId = user.TenantId,
        UserName = user.UserName,
        Email = user.Email,
        EmailConfirmed = user.EmailConfirmed,
        PhoneNumber = user.PhoneNumber,
        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
        TwoFactorEnabled = user.TwoFactorEnabled,
        LockoutEnabled = user.LockoutEnabled,
        LockoutEnd = user.LockoutEnd,
        AccessFailedCount = user.AccessFailedCount,
        Status = user.Status,
        Name = user.Name,
        Surname = user.Surname,
        ConcurrencyStamp = user.ConcurrencyStamp,
        CreationTime = user.CreationTime,
        CreatorId = user.CreatorId,
        LastModificationTime = user.LastModificationTime,
        LastModifierId = user.LastModifierId,
        IsDeleted = user.IsDeleted,
        DeletionTime = user.DeletionTime,
        DeleterId = user.DeleterId,
    };
}
