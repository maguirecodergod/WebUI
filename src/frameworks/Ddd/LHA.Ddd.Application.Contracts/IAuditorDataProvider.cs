namespace LHA.Ddd.Application;

/// <summary>
/// Abstraction for providing auditor data when a cache miss occurs.
/// Applications (e.g., Identity service) must implement and register this interface
/// to allow the framework to fetch missing user details.
/// </summary>
public interface IAuditorDataProvider
{
    /// <summary>
    /// Retrieves auditor details for the specified IDs.
    /// </summary>
    /// <param name="ids">A collection of user IDs.</param>
    /// <returns>A dictionary mapping user IDs to their respective <see cref="AuditorCacheItem"/>.</returns>
    Task<Dictionary<Guid, AuditorCacheItem>> GetAuditorsAsync(IEnumerable<Guid> ids);
}
