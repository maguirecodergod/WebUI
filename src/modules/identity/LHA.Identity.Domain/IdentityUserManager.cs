using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;

namespace LHA.Identity.Domain;

/// <summary>
/// Domain service responsible for creating and updating <see cref="IdentityUser"/> entities.
/// <para>
/// Encapsulates business rules that require repository access: username/email
/// uniqueness checks, password hashing, and lookup normalization.
/// </para>
/// </summary>
public sealed class IdentityUserManager : DomainService
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILookupNormalizer _lookupNormalizer;

    public IdentityUserManager(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ILookupNormalizer lookupNormalizer)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _lookupNormalizer = lookupNormalizer;
    }

    /// <summary>
    /// Creates a new user, validates uniqueness, hashes password,
    /// and assigns default roles.
    /// </summary>
    public async Task<IdentityUser> CreateAsync(
        string userName,
        string email,
        string password,
        Guid? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateAsync(userName, email, password, Guid.CreateVersion7(), tenantId, cancellationToken);
    }

    /// <summary>
    /// Creates a new user with a predetermined ID.
    /// Use this overload for seeding well-known users (e.g., admin, system) with deterministic IDs.
    /// </summary>
    public async Task<IdentityUser> CreateAsync(
        string userName,
        string email,
        string password,
        Guid id,
        Guid? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        await ValidateUserNameAsync(userName, existingUserId: null, cancellationToken);
        await ValidateEmailAsync(email, existingUserId: null, cancellationToken);

        var user = new IdentityUser(id, userName, email, tenantId);

        // Normalize
        user.SetNormalizedUserName(_lookupNormalizer.NormalizeName(userName));
        user.SetNormalizedEmail(_lookupNormalizer.NormalizeEmail(email));

        // Hash password
        user.SetPasswordHash(_passwordHasher.HashPassword(password));

        // Assign default roles
        var defaultRoles = await _roleRepository.GetDefaultRolesAsync(cancellationToken);
        foreach (var role in defaultRoles)
        {
            user.AddRole(role.Id);
        }

        return user;
    }

    /// <summary>
    /// Changes the user name with uniqueness validation.
    /// </summary>
    public async Task ChangeUserNameAsync(
        IdentityUser user,
        string newUserName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        await ValidateUserNameAsync(newUserName, user.Id, cancellationToken);

        user.SetUserName(newUserName);
        user.SetNormalizedUserName(_lookupNormalizer.NormalizeName(newUserName));
    }

    /// <summary>
    /// Changes the email with uniqueness validation.
    /// </summary>
    public async Task ChangeEmailAsync(
        IdentityUser user,
        string newEmail,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        await ValidateEmailAsync(newEmail, user.Id, cancellationToken);

        user.SetEmail(newEmail);
        user.SetNormalizedEmail(_lookupNormalizer.NormalizeEmail(newEmail));
    }

    /// <summary>
    /// Changes the user's password (hashes before storing).
    /// </summary>
    public void ChangePassword(IdentityUser user, string newPassword)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

        user.SetPasswordHash(_passwordHasher.HashPassword(newPassword));
    }

    /// <summary>
    /// Verifies a password against the user's stored hash.
    /// </summary>
    public bool VerifyPassword(IdentityUser user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        return _passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
    }

    // ─── Validation helpers ──────────────────────────────────────────

    private async Task ValidateUserNameAsync(
        string userName, Guid? existingUserId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        var normalized = _lookupNormalizer.NormalizeName(userName);
        var existing = await _userRepository.FindByNormalizedUserNameAsync(normalized, ct);

        if (existing is not null && existing.Id != existingUserId)
            throw new DuplicateException(IdentityDomainErrorCodes.DuplicateUserName)
                .WithData(userName);
    }

    private async Task ValidateEmailAsync(
        string email, Guid? existingUserId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalized = _lookupNormalizer.NormalizeEmail(email);
        var existing = await _userRepository.FindByNormalizedEmailAsync(normalized, ct);

        if (existing is not null && existing.Id != existingUserId)
            throw new DuplicateException(IdentityDomainErrorCodes.DuplicateEmail)
                .WithData(email);
    }
}
