namespace LHA.Ddd.Application;

/// <summary>
/// Service responsible for enriching DTOs with auditor (user) information using caching.
/// </summary>
public interface IAuditedDtoEnricher
{
    /// <summary>
    /// Enriches the audit fields (Creation, LastModification, Deletion) of the given object or collection of objects.
    /// </summary>
    Task EnrichAsync(object? obj);
}
