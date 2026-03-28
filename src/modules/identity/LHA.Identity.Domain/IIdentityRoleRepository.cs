using LHA.Core;
using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository abstraction for <see cref="IdentityRole"/> persistence.
/// </summary>
public interface IIdentityRoleRepository : IRepository<IdentityRole, Guid>
{
    /// <summary>Finds a role by normalized name (tenant-scoped).</summary>
    Task<IdentityRole?> FindByNormalizedNameAsync(
        string normalizedName, CancellationToken cancellationToken = default);

    /// <summary>Returns a filtered and paged list of roles.</summary>
    Task<List<IdentityRole>> GetListAsync(
        PagingParam paging,
        SorterParam? sorter = null,
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the total count matching the filter.</summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns all default roles for the current tenant.</summary>
    Task<List<IdentityRole>> GetDefaultRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets roles by their IDs.</summary>
    Task<List<IdentityRole>> GetListByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
