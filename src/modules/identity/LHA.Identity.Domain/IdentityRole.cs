using LHA.Core;
using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;
using LHA.MultiTenancy;

namespace LHA.Identity.Domain;

/// <summary>
/// The <c>IdentityRole</c> aggregate root.
/// <para>
/// Manages role metadata and role-level claims via domain methods.
/// Uniqueness of role name per tenant is validated by <see cref="IdentityRoleManager"/>.
/// </para>
/// </summary>
public sealed class IdentityRole : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    private readonly List<IdentityRoleClaim> _claims = [];

    /// <inheritdoc />
    public Guid? TenantId { get; private set; }

    /// <summary>Display name of the role.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Upper-cased name for lookups.</summary>
    public string NormalizedName { get; private set; } = null!;

    /// <summary>Active / Inactive status.</summary>
    public CMasterStatus Status { get; private set; }

    /// <summary>
    /// If <c>true</c>, new users are automatically assigned this role.
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// If <c>true</c>, the role cannot be deleted or renamed (system role).
    /// </summary>
    public bool IsStatic { get; private set; }

    /// <summary>
    /// If <c>true</c>, the role is visible across tenants (global role).
    /// </summary>
    public bool IsPublic { get; private set; }

    /// <summary>Claims associated with this role.</summary>
    public IReadOnlyCollection<IdentityRoleClaim> Claims => _claims.AsReadOnly();

    // ─── Constructors ────────────────────────────────────────────────

    /// <summary>EF Core constructor.</summary>
    private IdentityRole() { }

    /// <summary>
    /// Creates a new role. Called only by <see cref="IdentityRoleManager"/>.
    /// </summary>
    internal IdentityRole(
        Guid id,
        string name,
        Guid? tenantId = null,
        bool isDefault = false,
        bool isStatic = false,
        bool isPublic = false)
    {
        Id = id;
        TenantId = tenantId;
        Status = CMasterStatus.Active;
        IsDefault = isDefault;
        IsStatic = isStatic;
        IsPublic = isPublic;

        SetNameInternal(name);

        AddDomainEvent(new RoleCreatedDomainEvent(Id, Name, TenantId));
    }

    // ─── Name ────────────────────────────────────────────────────────

    /// <summary>
    /// Changes the role name. Cannot change a static role.
    /// Uniqueness must be validated by <see cref="IdentityRoleManager"/>.
    /// </summary>
    public IdentityRole SetName(string name)
    {
        if (IsStatic)
            throw new InvalidOperationException($"Cannot rename static role '{Name}'.");

        SetNameInternal(name);
        return this;
    }

    /// <summary>Sets the normalized name (used by lookup normalizer).</summary>
    internal IdentityRole SetNormalizedName(string normalizedName)
    {
        NormalizedName = normalizedName;
        return this;
    }

    // ─── Flags ───────────────────────────────────────────────────────

    /// <summary>Sets whether new users are automatically assigned this role.</summary>
    public IdentityRole SetIsDefault(bool isDefault)
    {
        IsDefault = isDefault;
        return this;
    }

    /// <summary>Sets public visibility.</summary>
    public IdentityRole SetIsPublic(bool isPublic)
    {
        IsPublic = isPublic;
        return this;
    }

    // ─── Activation ──────────────────────────────────────────────────

    /// <summary>Activates the role.</summary>
    public IdentityRole Activate()
    {
        if (Status == CMasterStatus.Active) return this;
        Status = CMasterStatus.Active;
        return this;
    }

    /// <summary>Deactivates the role.</summary>
    public IdentityRole Deactivate()
    {
        if (Status == CMasterStatus.InActive) return this;
        Status = CMasterStatus.InActive;
        return this;
    }

    // ─── Claims ──────────────────────────────────────────────────────

    /// <summary>Adds a claim to the role.</summary>
    public IdentityRole AddClaim(string claimType, string claimValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimValue);

        if (_claims.Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue))
            return this;

        _claims.Add(new IdentityRoleClaim(Id, claimType, claimValue, TenantId));
        return this;
    }

    /// <summary>Removes all claims with the given type.</summary>
    public IdentityRole RemoveClaims(string claimType)
    {
        _claims.RemoveAll(c => c.ClaimType == claimType);
        return this;
    }

    // ─── Internal helpers ────────────────────────────────────────────

    private void SetNameInternal(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > IdentityRoleConsts.MaxNameLength)
            throw new ArgumentException(
                $"Role name must not exceed {IdentityRoleConsts.MaxNameLength} characters.",
                nameof(name));

        Name = name.Trim();
        NormalizedName = Name.ToUpperInvariant();
    }
}

/// <summary>
/// Represents a claim associated with a role.
/// </summary>
public sealed class IdentityRoleClaim : Entity<Guid>, IMultiTenant
{
    public Guid RoleId { get; private init; }
    public string ClaimType { get; private set; } = null!;
    public string ClaimValue { get; private set; } = null!;
    public Guid? TenantId { get; private init; }

    private IdentityRoleClaim() { }

    internal IdentityRoleClaim(Guid roleId, string claimType, string claimValue, Guid? tenantId)
    {
        Id = Guid.CreateVersion7();
        RoleId = roleId;
        TenantId = tenantId;
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}
