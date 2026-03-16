using LHA.Ddd.Domain;

namespace LHA.Identity.Domain;

/// <summary>
/// Repository abstraction for <see cref="IdentitySecurityLog"/> persistence.
/// </summary>
public interface IIdentitySecurityLogRepository : IRepository<IdentitySecurityLog, Guid>
{
    /// <summary>Returns a filtered and paged list of security logs.</summary>
    Task<List<IdentitySecurityLog>> GetListAsync(
        string? filter = null,
        Guid? userId = null,
        string? action = null,
        string? sorting = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the total count matching the filter.</summary>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? userId = null,
        string? action = null,
        CancellationToken cancellationToken = default);
}
