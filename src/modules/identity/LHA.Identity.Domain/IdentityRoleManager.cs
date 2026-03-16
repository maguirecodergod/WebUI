using LHA.Ddd.Domain;
using LHA.Identity.Domain.Shared;

namespace LHA.Identity.Domain;

/// <summary>
/// Domain service responsible for creating and updating <see cref="IdentityRole"/> entities.
/// <para>
/// Encapsulates uniqueness validation for role names within a tenant.
/// </para>
/// </summary>
public sealed class IdentityRoleManager : DomainService
{
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly ILookupNormalizer _lookupNormalizer;

    public IdentityRoleManager(
        IIdentityRoleRepository roleRepository,
        ILookupNormalizer lookupNormalizer)
    {
        _roleRepository = roleRepository;
        _lookupNormalizer = lookupNormalizer;
    }

    /// <summary>
    /// Creates a new role with uniqueness validation.
    /// </summary>
    public async Task<IdentityRole> CreateAsync(
        string name,
        Guid? tenantId = null,
        bool isDefault = false,
        bool isStatic = false,
        bool isPublic = false,
        CancellationToken cancellationToken = default)
    {
        await ValidateNameAsync(name, existingRoleId: null, cancellationToken);

        var role = new IdentityRole(
            Guid.CreateVersion7(), name, tenantId, isDefault, isStatic, isPublic);

        role.SetNormalizedName(_lookupNormalizer.NormalizeName(name));
        return role;
    }

    /// <summary>
    /// Changes the role name with uniqueness validation.
    /// </summary>
    public async Task ChangeNameAsync(
        IdentityRole role,
        string newName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        await ValidateNameAsync(newName, role.Id, cancellationToken);

        role.SetName(newName);
        role.SetNormalizedName(_lookupNormalizer.NormalizeName(newName));
    }

    private async Task ValidateNameAsync(
        string name, Guid? existingRoleId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalized = _lookupNormalizer.NormalizeName(name);
        var existing = await _roleRepository.FindByNormalizedNameAsync(normalized, ct);

        if (existing is not null && existing.Id != existingRoleId)
            throw new DuplicateException(IdentityDomainErrorCodes.DuplicateRoleName)
                .WithData(name);
    }
}
