using LHA.Core;
using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;

namespace LHA.Identity.Domain;

/// <summary>
/// The <c>IdentityUser</c> aggregate root — core identity entity.
/// <para>
/// All mutations go through domain methods that enforce invariants
/// (unique email/username validated by <see cref="IdentityUserManager"/>)
/// and raise domain events.
/// </para>
/// </summary>
public sealed class IdentityUser : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    private readonly List<IdentityUserRole> _roles = [];
    private readonly List<IdentityUserClaim> _claims = [];
    private readonly List<IdentityUserLogin> _logins = [];
    private readonly List<IdentityUserToken> _tokens = [];

    // ─── Tenant ──────────────────────────────────────────────────────

    /// <inheritdoc />
    public Guid? TenantId { get; private set; }

    // ─── Credentials & Identity ──────────────────────────────────────

    /// <summary>Unique login name.</summary>
    public string UserName { get; private set; } = null!;

    /// <summary>Upper-cased user name for case-insensitive lookups.</summary>
    public string NormalizedUserName { get; private set; } = null!;

    /// <summary>Email address.</summary>
    public string Email { get; private set; } = null!;

    /// <summary>Upper-cased email for case-insensitive lookups.</summary>
    public string NormalizedEmail { get; private set; } = null!;

    /// <summary>Whether the email has been confirmed.</summary>
    public bool EmailConfirmed { get; private set; }

    /// <summary>Hashed password (BCrypt or PBKDF2).</summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>
    /// Random value that changes whenever credentials change
    /// (password, roles, logins, 2FA). Used for cache-busting and token invalidation.
    /// </summary>
    public string SecurityStamp { get; private set; } = null!;

    // ─── Phone ───────────────────────────────────────────────────────

    /// <summary>Phone number (optional).</summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>Whether the phone number has been confirmed.</summary>
    public bool PhoneNumberConfirmed { get; private set; }

    // ─── Two-Factor ──────────────────────────────────────────────────

    /// <summary>Whether two-factor authentication is enabled.</summary>
    public bool TwoFactorEnabled { get; private set; }

    // ─── Lockout ─────────────────────────────────────────────────────

    /// <summary>UTC time when lockout ends. <c>null</c> means not locked out.</summary>
    public DateTimeOffset? LockoutEnd { get; private set; }

    /// <summary>Whether lockout is enabled for this user.</summary>
    public bool LockoutEnabled { get; private set; } = true;

    /// <summary>Number of consecutive failed login attempts.</summary>
    public int AccessFailedCount { get; private set; }

    // ─── Status ──────────────────────────────────────────────────────

    /// <summary>Active / Inactive status.</summary>
    public CMasterStatus Status { get; private set; }

    // ─── Profile ─────────────────────────────────────────────────────

    /// <summary>First name.</summary>
    public string? Name { get; private set; }

    /// <summary>Last name.</summary>
    public string? Surname { get; private set; }

    // ─── Navigation collections (private backing fields) ─────────────

    /// <summary>Roles assigned to this user.</summary>
    public IReadOnlyCollection<IdentityUserRole> Roles => _roles.AsReadOnly();

    /// <summary>Claims associated with this user.</summary>
    public IReadOnlyCollection<IdentityUserClaim> Claims => _claims.AsReadOnly();

    /// <summary>External login providers linked to this user.</summary>
    public IReadOnlyCollection<IdentityUserLogin> Logins => _logins.AsReadOnly();

    /// <summary>Tokens (refresh tokens, authenticator tokens, etc.).</summary>
    public IReadOnlyCollection<IdentityUserToken> Tokens => _tokens.AsReadOnly();

    // ─── Constructors ────────────────────────────────────────────────

    /// <summary>EF Core constructor.</summary>
    private IdentityUser() { }

    /// <summary>
    /// Creates a new identity user. Called only by <see cref="IdentityUserManager"/>.
    /// </summary>
    internal IdentityUser(
        Guid id,
        string userName,
        string email,
        Guid? tenantId = null)
    {
        Id = id;
        TenantId = tenantId;
        SecurityStamp = Guid.NewGuid().ToString("N");
        Status = CMasterStatus.Active;

        SetUserNameInternal(userName);
        SetEmailInternal(email);

        AddDomainEvent(new UserCreatedDomainEvent(Id, UserName, Email, TenantId));
    }

    // ─── UserName ────────────────────────────────────────────────────

    /// <summary>
    /// Changes the user name. Uniqueness must be validated by <see cref="IdentityUserManager"/>.
    /// </summary>
    public IdentityUser SetUserName(string userName)
    {
        SetUserNameInternal(userName);
        return this;
    }

    /// <summary>Sets the normalized user name (used by lookup normalizer).</summary>
    internal IdentityUser SetNormalizedUserName(string normalizedUserName)
    {
        NormalizedUserName = normalizedUserName;
        return this;
    }

    // ─── Email ───────────────────────────────────────────────────────

    /// <summary>
    /// Changes the email. Uniqueness must be validated by <see cref="IdentityUserManager"/>.
    /// </summary>
    public IdentityUser SetEmail(string email)
    {
        SetEmailInternal(email);
        EmailConfirmed = false;
        return this;
    }

    /// <summary>Sets the normalized email (used by lookup normalizer).</summary>
    internal IdentityUser SetNormalizedEmail(string normalizedEmail)
    {
        NormalizedEmail = normalizedEmail;
        return this;
    }

    /// <summary>Marks the email as confirmed.</summary>
    public IdentityUser ConfirmEmail()
    {
        EmailConfirmed = true;
        return this;
    }

    // ─── Password ────────────────────────────────────────────────────

    /// <summary>Sets the password hash. Called by <see cref="IdentityUserManager"/>.</summary>
    internal IdentityUser SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        RotateSecurityStamp();
        AddDomainEvent(new UserPasswordChangedDomainEvent(Id));
        return this;
    }

    // ─── Phone ───────────────────────────────────────────────────────

    /// <summary>Sets the phone number.</summary>
    public IdentityUser SetPhoneNumber(string? phoneNumber)
    {
        if (phoneNumber?.Length > IdentityUserConsts.MaxPhoneNumberLength)
            throw new ArgumentException(
                $"Phone number must not exceed {IdentityUserConsts.MaxPhoneNumberLength} characters.",
                nameof(phoneNumber));

        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = false;
        return this;
    }

    /// <summary>Marks the phone number as confirmed.</summary>
    public IdentityUser ConfirmPhoneNumber()
    {
        PhoneNumberConfirmed = true;
        return this;
    }

    // ─── Profile ─────────────────────────────────────────────────────

    /// <summary>Sets the display name (first name).</summary>
    public IdentityUser SetName(string? name)
    {
        if (name?.Length > IdentityUserConsts.MaxNameLength)
            throw new ArgumentException(
                $"Name must not exceed {IdentityUserConsts.MaxNameLength} characters.", nameof(name));
        Name = name;
        return this;
    }

    /// <summary>Sets the surname (last name).</summary>
    public IdentityUser SetSurname(string? surname)
    {
        if (surname?.Length > IdentityUserConsts.MaxSurnameLength)
            throw new ArgumentException(
                $"Surname must not exceed {IdentityUserConsts.MaxSurnameLength} characters.", nameof(surname));
        Surname = surname;
        return this;
    }

    // ─── Two-Factor ──────────────────────────────────────────────────

    /// <summary>Enables or disables two-factor authentication.</summary>
    public IdentityUser SetTwoFactorEnabled(bool enabled)
    {
        TwoFactorEnabled = enabled;
        RotateSecurityStamp();
        return this;
    }

    // ─── Lockout ─────────────────────────────────────────────────────

    /// <summary>Sets whether lockout is enabled for this user.</summary>
    public IdentityUser SetLockoutEnabled(bool enabled)
    {
        LockoutEnabled = enabled;
        return this;
    }

    /// <summary>Records a failed login attempt. Returns the new failed count.</summary>
    public int IncrementAccessFailedCount()
    {
        AccessFailedCount++;
        return AccessFailedCount;
    }

    /// <summary>Resets the failed login count and clears any lockout.</summary>
    public IdentityUser ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
        LockoutEnd = null;
        return this;
    }

    /// <summary>Locks the user out until the specified time.</summary>
    public IdentityUser LockUntil(DateTimeOffset lockoutEnd)
    {
        LockoutEnd = lockoutEnd;
        AddDomainEvent(new UserLockedOutDomainEvent(Id, lockoutEnd));
        return this;
    }

    /// <summary>Returns <c>true</c> if the user is currently locked out.</summary>
    public bool IsLockedOut => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;

    // ─── Activation ──────────────────────────────────────────────────

    /// <summary>Activates the user.</summary>
    public IdentityUser Activate()
    {
        if (Status == CMasterStatus.Active) return this;
        Status = CMasterStatus.Active;
        return this;
    }

    /// <summary>Deactivates the user.</summary>
    public IdentityUser Deactivate()
    {
        if (Status == CMasterStatus.InActive) return this;
        Status = CMasterStatus.InActive;
        return this;
    }

    // ─── Roles ───────────────────────────────────────────────────────

    /// <summary>Adds a role if not already assigned.</summary>
    public IdentityUser AddRole(Guid roleId)
    {
        if (_roles.Any(r => r.RoleId == roleId)) return this;

        _roles.Add(new IdentityUserRole(Id, roleId, TenantId));
        RotateSecurityStamp();
        AddDomainEvent(new UserRoleChangedDomainEvent(Id, roleId, IsAdded: true));
        return this;
    }

    /// <summary>Removes a role.</summary>
    public IdentityUser RemoveRole(Guid roleId)
    {
        var existing = _roles.FirstOrDefault(r => r.RoleId == roleId);
        if (existing is null) return this;

        _roles.Remove(existing);
        RotateSecurityStamp();
        AddDomainEvent(new UserRoleChangedDomainEvent(Id, roleId, IsAdded: false));
        return this;
    }

    /// <summary>Checks if the user has a specific role.</summary>
    public bool HasRole(Guid roleId) => _roles.Any(r => r.RoleId == roleId);

    // ─── Claims ──────────────────────────────────────────────────────

    /// <summary>Adds a claim.</summary>
    public IdentityUser AddClaim(string claimType, string claimValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimValue);

        if (_claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue))
            return this;

        _claims.Add(new IdentityUserClaim(Id, claimType, claimValue, TenantId));
        return this;
    }

    /// <summary>Removes all claims with the given type.</summary>
    public IdentityUser RemoveClaims(string claimType)
    {
        _claims.RemoveAll(c => c.ClaimType == claimType);
        return this;
    }

    /// <summary>Replaces all claims of a given type with a new value.</summary>
    public IdentityUser ReplaceClaim(string claimType, string oldValue, string newValue)
    {
        var existing = _claims.FirstOrDefault(c => c.ClaimType == claimType && c.ClaimValue == oldValue);
        if (existing is not null)
        {
            _claims.Remove(existing);
            _claims.Add(new IdentityUserClaim(Id, claimType, newValue, TenantId));
        }
        return this;
    }

    // ─── Logins ──────────────────────────────────────────────────────

    /// <summary>Adds an external login provider link.</summary>
    public IdentityUser AddLogin(string loginProvider, string providerKey, string? providerDisplayName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);

        if (_logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey))
            return this;

        _logins.Add(new IdentityUserLogin(Id, loginProvider, providerKey, providerDisplayName, TenantId));
        RotateSecurityStamp();
        return this;
    }

    /// <summary>Removes an external login provider link.</summary>
    public IdentityUser RemoveLogin(string loginProvider, string providerKey)
    {
        var existing = _logins.FirstOrDefault(
            l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
        if (existing is not null) _logins.Remove(existing);
        return this;
    }

    // ─── Tokens ──────────────────────────────────────────────────────

    /// <summary>Sets (adds or updates) a user token.</summary>
    public IdentityUser SetToken(string loginProvider, string name, string value, DateTimeOffset? expiresAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loginProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var existing = _tokens.FirstOrDefault(
            t => t.LoginProvider == loginProvider && t.Name == name);

        if (existing is not null)
        {
            existing.SetValue(value, expiresAt);
        }
        else
        {
            _tokens.Add(new IdentityUserToken(Id, loginProvider, name, value, expiresAt, TenantId));
        }

        return this;
    }

    /// <summary>Removes a user token.</summary>
    public IdentityUser RemoveToken(string loginProvider, string name)
    {
        var existing = _tokens.FirstOrDefault(
            t => t.LoginProvider == loginProvider && t.Name == name);
        if (existing is not null) _tokens.Remove(existing);
        return this;
    }

    /// <summary>Finds a token value.</summary>
    public string? FindTokenValue(string loginProvider, string name)
        => _tokens.FirstOrDefault(
            t => t.LoginProvider == loginProvider && t.Name == name)?.Value;

    // ─── Security Stamp ──────────────────────────────────────────────

    /// <summary>Generates a new security stamp (invalidates existing tokens).</summary>
    public IdentityUser RotateSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid().ToString("N");
        return this;
    }

    // ─── Internal helpers ────────────────────────────────────────────

    private void SetUserNameInternal(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        if (userName.Length > IdentityUserConsts.MaxUserNameLength)
            throw new ArgumentException(
                $"User name must not exceed {IdentityUserConsts.MaxUserNameLength} characters.",
                nameof(userName));

        UserName = userName.Trim();
        NormalizedUserName = UserName.ToUpperInvariant();
    }

    private void SetEmailInternal(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (email.Length > IdentityUserConsts.MaxEmailLength)
            throw new ArgumentException(
                $"Email must not exceed {IdentityUserConsts.MaxEmailLength} characters.",
                nameof(email));

        Email = email.Trim();
        NormalizedEmail = Email.ToUpperInvariant();
    }
}
