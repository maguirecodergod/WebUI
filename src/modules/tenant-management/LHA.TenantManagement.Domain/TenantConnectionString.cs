using LHA.Ddd.Domain;
using LHA.Shared.Domain.TenantManagement;

namespace LHA.TenantManagement.Domain;

/// <summary>
/// Represents a connection string associated with a specific tenant.
/// Managed exclusively through the <see cref="Tenant"/> aggregate root.
/// </summary>
public sealed class TenantConnectionString : Entity<Guid>
{
    /// <summary>Foreign key to the owning <see cref="Tenant"/>.</summary>
    public Guid TenantId { get; private init; }

    /// <summary>Logical connection string name (e.g., "Default").</summary>
    public string Name { get; private set; } = null!;

    /// <summary>The actual connection string value.</summary>
    public string Value { get; private set; } = null!;

    /// <summary>EF Core constructor.</summary>
    private TenantConnectionString() { }

    internal TenantConnectionString(Guid tenantId, string name, string value)
    {
        Id = Guid.CreateVersion7();
        TenantId = tenantId;
        SetName(name);
        SetValue(value);
    }

    internal void SetValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > TenantConsts.MaxConnectionStringValueLength)
            throw new ArgumentException(
                $"Connection string value must not exceed {TenantConsts.MaxConnectionStringValueLength} characters.",
                nameof(value));

        Value = value;
    }

    private void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > TenantConsts.MaxConnectionStringNameLength)
            throw new ArgumentException(
                $"Connection string name must not exceed {TenantConsts.MaxConnectionStringNameLength} characters.",
                nameof(name));

        Name = name.Trim();
    }
}
