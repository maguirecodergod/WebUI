using System.Security.Claims;
using LHA.Core.Users;
using LHA.Core.Security;
using Microsoft.AspNetCore.Http;

namespace LHA.AspNetCore.Security;

/// <summary>
/// <see cref="ICurrentUser"/> implementation that reads claims from
/// <see cref="HttpContext.User"/> (<see cref="ClaimsPrincipal"/>).
/// <para>
/// Registered as <b>Scoped</b> — one instance per HTTP request.
/// Falls back to <see cref="ICurrentUserAccessor"/> for non-HTTP contexts
/// (e.g., background tasks running within an HTTP-scoped DI scope).
/// </para>
/// </summary>
internal sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal? _principal;
    private readonly BasicUserInfo? _accessorInfo;

    /// <summary>
    /// Cached roles (lazy, read once from claims).
    /// </summary>
    private string[]? _cachedRoles;

    public HttpContextCurrentUser(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserAccessor currentUserAccessor)
    {
        _principal = httpContextAccessor.HttpContext?.User;
        _accessorInfo = currentUserAccessor.Current;
    }

    /// <inheritdoc />
    public bool IsAuthenticated =>
        _principal?.Identity?.IsAuthenticated == true || _accessorInfo is not null;

    /// <inheritdoc />
    public Guid? Id
    {
        get
        {
            // 1) Try HttpContext claims
            var sub = _principal?.FindFirstValue(LhaClaimTypes.Subject)
                   ?? _principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(sub, out var id))
                return id;

            // 2) Fallback to AsyncLocal accessor
            return _accessorInfo?.UserId;
        }
    }

    /// <inheritdoc />
    public string? UserName =>
        _principal?.FindFirstValue(LhaClaimTypes.PreferredUserName)
        ?? _principal?.FindFirstValue(ClaimTypes.Name)
        ?? _accessorInfo?.UserName;

    /// <inheritdoc />
    public string? Name =>
        _principal?.FindFirstValue(LhaClaimTypes.GivenName)
        ?? _principal?.FindFirstValue(ClaimTypes.GivenName);

    /// <inheritdoc />
    public string? Surname =>
        _principal?.FindFirstValue(LhaClaimTypes.FamilyName)
        ?? _principal?.FindFirstValue(ClaimTypes.Surname);

    /// <inheritdoc />
    public string? Email =>
        _principal?.FindFirstValue(LhaClaimTypes.Email)
        ?? _principal?.FindFirstValue(ClaimTypes.Email);

    /// <inheritdoc />
    public bool EmailVerified =>
        string.Equals(
            _principal?.FindFirstValue(LhaClaimTypes.EmailVerified),
            "true", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public string? PhoneNumber =>
        _principal?.FindFirstValue(LhaClaimTypes.PhoneNumber)
        ?? _principal?.FindFirstValue(ClaimTypes.MobilePhone);

    /// <inheritdoc />
    public bool PhoneNumberVerified =>
        string.Equals(
            _principal?.FindFirstValue(LhaClaimTypes.PhoneNumberVerified),
            "true", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public Guid? TenantId
    {
        get
        {
            var tenantClaim = _principal?.FindFirstValue(LhaClaimTypes.TenantId);
            if (Guid.TryParse(tenantClaim, out var tid))
                return tid;

            return _accessorInfo?.TenantId;
        }
    }

    /// <inheritdoc />
    public string[] Roles
    {
        get
        {
            if (_cachedRoles is not null) return _cachedRoles;

            // Combine from claims
            var fromClaims = _principal?.FindAll(LhaClaimTypes.Role)
                .Concat(_principal?.FindAll(ClaimTypes.Role) ?? [])
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? [];

            // Merge with accessor roles
            if (_accessorInfo?.Roles is { Length: > 0 } accessorRoles)
            {
                fromClaims = fromClaims
                    .Concat(accessorRoles)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            _cachedRoles = fromClaims;
            return _cachedRoles;
        }
    }

    /// <inheritdoc />
    public string? FindClaimValue(string claimType) =>
        _principal?.FindFirstValue(claimType);

    /// <inheritdoc />
    public string[] FindClaimValues(string claimType) =>
        _principal?.FindAll(claimType).Select(c => c.Value).ToArray() ?? [];

    /// <inheritdoc />
    public bool IsInRole(string roleName) =>
        Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
}
