using LHA.Auditing;
using LHA.Core;
using LHA.Ddd.Application;
using LHA.EventBus;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Domain;
using LHA.Identity.Domain.Shared;
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
        IClientInfoProvider clientInfoProvider)
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

        // Success — reset failed count, generate tokens
        user.ResetAccessFailedCount();

        var (roleIds, roleNames) = await GetRolesAsync(user, ct);
        var permissions = await ResolvePermissionsAsync(user.Id, roleIds, ct);
        var accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id, user.UserName, user.Email, user.TenantId, roleNames, permissions);
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
        var (roleIds, roleNames) = await GetRolesAsync(user, ct);
        var permissions = await ResolvePermissionsAsync(user.Id, roleIds, ct);
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
        var (_, roleNames) = await GetRolesAsync(user, ct);

        return new CurrentUserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            UserName = user.UserName,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname,
            Roles = [.. roleNames],
        };
    }

    // ─── Helpers ─────────────────────────────────────────────────────

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
        IdentityUser user, CancellationToken ct)
    {
        var roleIds = user.Roles.Select(r => r.RoleId).ToList();
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
        Guid userId, List<Guid> roleIds, CancellationToken ct)
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
