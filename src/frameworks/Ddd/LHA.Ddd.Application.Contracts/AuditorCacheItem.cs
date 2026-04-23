namespace LHA.Ddd.Application;

/// <summary>
/// A standardized cache item to store auditor (user) identity information.
/// Used by the framework to enrich audit DTOs efficiently.
/// </summary>
public class AuditorCacheItem
{
    /// <summary>
    /// Identifier of the auditor.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the auditor.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The avatar of the auditor.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// The email of the auditor.
    /// </summary>
    public string? Email { get; set; }
}
