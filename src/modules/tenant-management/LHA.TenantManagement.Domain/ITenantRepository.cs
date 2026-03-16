using LHA.Core;
using LHA.Ddd.Domain;

namespace LHA.TenantManagement.Domain;

/// <summary>
/// Repository abstraction for <see cref="TenantEntity"/> persistence.
/// Extends the generic <see cref="IRepository{TEntity,TKey}"/> with tenant-specific queries.
/// </summary>
public interface ITenantRepository : IRepository<TenantEntity, Guid>
{
    /// <summary>
    /// Finds a tenant by its normalized (upper-cased) name.
    /// Returns <see langword="null"/> if not found.
    /// </summary>
    Task<TenantEntity?> FindByNameAsync(
        string normalizedName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a filtered and paged list of tenants.
    /// </summary>
    Task<List<TenantEntity>> GetListAsync(
        string? filter = null,
        CMasterStatus? status = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count matching the filter.
    /// </summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CMasterStatus? status = null,
        CancellationToken cancellationToken = default);
}
