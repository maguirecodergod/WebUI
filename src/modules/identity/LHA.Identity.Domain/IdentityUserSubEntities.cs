using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;

namespace LHA.Identity.Domain;

/// <summary>
/// Represents a user–role assignment (many-to-many join entity).
/// </summary>
public sealed class IdentityUserRole : Entity<Guid>, IMultiTenant
{
    public Guid UserId { get; private init; }
    public Guid RoleId { get; private init; }
    public Guid? TenantId { get; private init; }

    private IdentityUserRole() { }

    internal IdentityUserRole(Guid userId, Guid roleId, Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        RoleId = roleId;
        TenantId = tenantId;
    }
}

/// <summary>
/// Represents a claim associated with a user.
/// </summary>
public sealed class IdentityUserClaim : Entity<Guid>, IMultiTenant
{
    public Guid UserId { get; private init; }
    public string ClaimType { get; private set; } = null!;
    public string ClaimValue { get; private set; } = null!;
    public Guid? TenantId { get; private init; }

    private IdentityUserClaim() { }

    internal IdentityUserClaim(Guid userId, string claimType, string claimValue, Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TenantId = tenantId;

        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        if (claimType.Length > IdentityClaimConsts.MaxClaimTypeLength)
            throw new ArgumentException($"Claim type must not exceed {IdentityClaimConsts.MaxClaimTypeLength} characters.");

        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}

/// <summary>
/// Represents an external login provider (OAuth / OpenID Connect) linked to a user.
/// </summary>
public sealed class IdentityUserLogin : Entity<Guid>, IMultiTenant
{
    public Guid UserId { get; private init; }
    public string LoginProvider { get; private init; } = null!;
    public string ProviderKey { get; private init; } = null!;
    public string? ProviderDisplayName { get; private set; }
    public Guid? TenantId { get; private init; }

    private IdentityUserLogin() { }

    internal IdentityUserLogin(
        Guid userId, string loginProvider, string providerKey,
        string? providerDisplayName, Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TenantId = tenantId;

        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        LoginProvider = loginProvider;
        ProviderKey = providerKey;
        ProviderDisplayName = providerDisplayName;
    }
}

/// <summary>
/// Represents a token stored for a user (refresh token, authenticator key, etc.).
/// </summary>
public sealed class IdentityUserToken : Entity<Guid>, IMultiTenant
{
    public Guid UserId { get; private init; }
    public string LoginProvider { get; private init; } = null!;
    public string Name { get; private init; } = null!;
    public string Value { get; private set; } = null!;
    public DateTimeOffset? ExpiresAt { get; private set; }
    public Guid? TenantId { get; private init; }

    private IdentityUserToken() { }

    internal IdentityUserToken(
        Guid userId, string loginProvider, string name, string value,
        DateTimeOffset? expiresAt, Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TenantId = tenantId;

        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        LoginProvider = loginProvider;
        Name = name;
        Value = value;
        ExpiresAt = expiresAt;
    }

    /// <summary>Updates the token value and optional expiration.</summary>
    internal void SetValue(string value, DateTimeOffset? expiresAt = null)
    {
        Value = value;
        ExpiresAt = expiresAt;
    }
}
