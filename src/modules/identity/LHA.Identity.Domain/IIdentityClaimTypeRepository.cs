using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository abstraction for <see cref="IdentityClaimType"/> persistence.
/// </summary>
public interface IIdentityClaimTypeRepository : IRepository<IdentityClaimType, Guid>
{
    /// <summary>Checks if a claim type name already exists.</summary>
    Task<bool> AnyAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>Returns a paged list of claim types.</summary>
    Task<List<IdentityClaimType>> GetListAsync(
        string? filter = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the total count matching the filter.</summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);
}
