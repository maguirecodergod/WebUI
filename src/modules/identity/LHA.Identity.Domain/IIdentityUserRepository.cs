using LHA.Core;
using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository abstraction for <see cref="IdentityUser"/> persistence.
/// </summary>
public interface IIdentityUserRepository : IRepository<IdentityUser, Guid>
{
    /// <summary>Finds a user by normalized user name (tenant-scoped).</summary>
    Task<IdentityUser?> FindByNormalizedUserNameAsync(
        string normalizedUserName, CancellationToken cancellationToken = default);

    /// <summary>Finds a user by normalized email (tenant-scoped).</summary>
    Task<IdentityUser?> FindByNormalizedEmailAsync(
        string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>Finds a user by external login.</summary>
    Task<IdentityUser?> FindByLoginAsync(
        string loginProvider, string providerKey, CancellationToken cancellationToken = default);

    /// <summary>Finds a user by a stored token value (e.g. refresh token).</summary>
    Task<IdentityUser?> FindByTokenAsync(
        string loginProvider, string name, string value, CancellationToken cancellationToken = default);

    /// <summary>Returns a filtered and paged list of users.</summary>
    Task<List<IdentityUser>> GetListAsync(
        string? filter = null,
        CMasterStatus? status = null,
        Guid? roleId = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the total count matching the filter.</summary>
    Task<long> GetCountAsync(
        string? filter = null,
        CMasterStatus? status = null,
        Guid? roleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Gets all role IDs assigned to a user.</summary>
    Task<List<Guid>> GetRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Gets all users in a specific role.</summary>
    Task<List<IdentityUser>> GetUsersInRoleAsync(
        Guid roleId, CancellationToken cancellationToken = default);
}
