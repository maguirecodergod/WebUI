using LHA.Core;
using LHA.Ddd.Domain;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.TenantManagement.Domain;

/// <summary>
/// The Tenant aggregate root — the core entity of the Tenant Management bounded context.
/// <para>
/// All mutations go through domain methods that enforce invariants and raise domain events.
/// </para>
/// </summary>
public sealed class TenantEntity : FullAuditedAggregateRoot<Guid>
{
    private readonly List<TenantConnectionString> _connectionStrings = [];

    // ─── Properties (private set = rich domain model) ────────────────

    /// <summary>Tenant display name.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Upper-cased name for case-insensitive lookups.</summary>
    public string NormalizedName { get; private set; } = null!;

    /// <summary>Whether the tenant is active and accepting requests.</summary>
    public CMasterStatus Status { get; private set; }

    /// <summary>Database isolation strategy for this tenant.</summary>
    public CMultiTenancyDatabaseStyle DatabaseStyle { get; private set; }

    /// <summary>Connection strings owned by this tenant.</summary>
    public IReadOnlyCollection<TenantConnectionString> ConnectionStrings
        => _connectionStrings.AsReadOnly();

    // ─── Constructors ────────────────────────────────────────────────

    /// <summary>EF Core constructor.</summary>
    private TenantEntity() { }

    /// <summary>
    /// Creates a new tenant. Called only by <see cref="TenantManager"/>.
    /// </summary>
    internal TenantEntity(
        Guid id,
        string name,
        CMultiTenancyDatabaseStyle databaseStyle = CMultiTenancyDatabaseStyle.Shared)
    {
        Id = id;
        SetNameInternal(name);
        Status = CMasterStatus.Active;
        DatabaseStyle = databaseStyle;

        AddDomainEvent(new TenantCreatedDomainEvent(Id, Name));
    }

    // ─── Name ────────────────────────────────────────────────────────

    /// <summary>
    /// Changes the tenant name. Raises <see cref="TenantNameChangedDomainEvent"/>.
    /// Uniqueness across tenants must be validated by <see cref="TenantManager"/>.
    /// </summary>
    public TenantEntity SetName(string name)
    {
        if (string.Equals(Name, name?.Trim(), StringComparison.Ordinal))
            return this;

        var oldName = Name;
        SetNameInternal(name!);

        AddDomainEvent(new TenantNameChangedDomainEvent(Id, oldName, Name));
        return this;
    }

    // ─── Activation ──────────────────────────────────────────────────

    /// <summary>Activates the tenant. Raises <see cref="TenantActivationChangedDomainEvent"/>.</summary>
    public TenantEntity Activate()
    {
        if (Status == CMasterStatus.Active) return this;

        Status = CMasterStatus.Active;
        AddDomainEvent(new TenantActivationChangedDomainEvent(Id, true));
        return this;
    }

    /// <summary>Deactivates the tenant. Raises <see cref="TenantActivationChangedDomainEvent"/>.</summary>
    public TenantEntity Deactivate()
    {
        if (Status == CMasterStatus.InActive) return this;

        Status = CMasterStatus.InActive;
        AddDomainEvent(new TenantActivationChangedDomainEvent(Id, false));
        return this;
    }

    // ─── Database Style ──────────────────────────────────────────────

    /// <summary>Changes the database isolation strategy.</summary>
    public TenantEntity SetDatabaseStyle(CMultiTenancyDatabaseStyle style)
    {
        DatabaseStyle = style;
        return this;
    }

    // ─── Connection Strings ──────────────────────────────────────────

    /// <summary>
    /// Adds or updates a named connection string.
    /// Raises <see cref="TenantConnectionStringChangedDomainEvent"/>.
    /// </summary>
    public TenantConnectionString AddOrUpdateConnectionString(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var existing = _connectionStrings.FirstOrDefault(
            cs => string.Equals(cs.Name, name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            existing.SetValue(value);
        }
        else
        {
            existing = new TenantConnectionString(Id, name, value);
            _connectionStrings.Add(existing);
        }

        AddDomainEvent(new TenantConnectionStringChangedDomainEvent(Id, name));
        return existing;
    }

    /// <summary>
    /// Removes a named connection string.
    /// Raises <see cref="TenantConnectionStringChangedDomainEvent"/> if found.
    /// </summary>
    public TenantEntity RemoveConnectionString(string name)
    {
        var existing = _connectionStrings.FirstOrDefault(
            cs => string.Equals(cs.Name, name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            _connectionStrings.Remove(existing);
            AddDomainEvent(new TenantConnectionStringChangedDomainEvent(Id, name));
        }

        return this;
    }

    /// <summary>Finds a connection string value by logical name.</summary>
    public string? FindConnectionString(string name)
        => _connectionStrings.FirstOrDefault(
            cs => string.Equals(cs.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;

    /// <summary>Finds the default connection string.</summary>
    public string? FindDefaultConnectionString()
        => FindConnectionString(TenantConsts.DefaultConnectionStringName);

    // ─── Internal helpers ────────────────────────────────────────────

    private void SetNameInternal(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > TenantConsts.MaxNameLength)
            throw new ArgumentException(
                $"Tenant name must not exceed {TenantConsts.MaxNameLength} characters.",
                nameof(name));

        Name = name.Trim();
        NormalizedName = Name.ToUpperInvariant();
    }
}
