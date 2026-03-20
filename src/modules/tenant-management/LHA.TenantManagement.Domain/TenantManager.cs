using LHA.Ddd.Domain;
using LHA.TenantManagement.Domain.Shared;

namespace LHA.TenantManagement.Domain;

/// <summary>
/// Domain service that encapsulates tenant creation and cross-aggregate validation
/// (e.g., unique name enforcement).
/// </summary>
public sealed class TenantManager : DomainService
{
    private readonly ITenantRepository _tenantRepository;

    public TenantManager(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    /// <summary>
    /// Creates a new tenant with unique name validation.
    /// </summary>
    /// <param name="name">The tenant display name.</param>
    /// <param name="databaseStyle">Database isolation strategy.</param>
    /// <returns>The newly created <see cref="TenantEntity"/> (not yet persisted).</returns>
    /// <exception cref="InvalidOperationException">When a tenant with the same name already exists.</exception>
    public async Task<TenantEntity> CreateAsync(
        string name,
        CMultiTenancyDatabaseStyle databaseStyle = CMultiTenancyDatabaseStyle.Shared,
        CancellationToken cancellationToken = default)
    {
        await ValidateNameAsync(name, existingTenantId: null, cancellationToken);

        return new TenantEntity(Guid.CreateVersion7(), name, databaseStyle);
    }

    /// <summary>
    /// Changes a tenant's name, enforcing uniqueness across all tenants.
    /// </summary>
    public async Task ChangeNameAsync(
        TenantEntity tenant,
        string newName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        if (string.Equals(tenant.NormalizedName, newName.Trim().ToUpperInvariant(), StringComparison.Ordinal))
            return;

        await ValidateNameAsync(newName, tenant.Id, cancellationToken);
        tenant.SetName(newName);
    }

    // ─── Private ─────────────────────────────────────────────────────

    private async Task ValidateNameAsync(
        string name,
        Guid? existingTenantId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim().ToUpperInvariant();
        var existing = await _tenantRepository.FindByNameAsync(normalizedName, cancellationToken);

        if (existing is not null && existing.Id != existingTenantId)
            throw new InvalidOperationException(
                $"A tenant with name '{name}' already exists.");
    }
}
