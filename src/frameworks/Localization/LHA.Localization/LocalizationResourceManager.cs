using System.Collections.Concurrent;
using LHA.Localization.Abstraction;
using Microsoft.Extensions.Options;

namespace LHA.Localization;

/// <summary>
/// Central manager for all registered localization resources.
/// Resolves and caches localization dictionaries per resource type and culture.
/// Supports resource inheritance (base resource fallback) and contributor merging.
/// </summary>
public sealed class LocalizationResourceManager
{
    private readonly LocalizationResourceOptions _options;
    private readonly ILocalizationResourceReader _reader;
    private readonly IEnumerable<ILocalizationResourceContributor> _contributors;

    /// <summary>
    /// Cache: ResourceType -> Culture -> Dictionary of strings.
    /// </summary>
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, IReadOnlyDictionary<string, string>>> _cache = new();

    public LocalizationResourceManager(
        IOptions<LocalizationResourceOptions> options,
        ILocalizationResourceReader reader,
        IEnumerable<ILocalizationResourceContributor> contributors)
    {
        _options = options.Value;
        _reader = reader;
        _contributors = contributors;
    }

    /// <summary>
    /// Gets the resource descriptor for the given resource type.
    /// </summary>
    public LocalizationResourceDescriptor? GetDescriptor(Type resourceType)
    {
        return _options.Resources.FirstOrDefault(r => r.ResourceType == resourceType);
    }

    /// <summary>
    /// Resolves all localization strings for a resource type and culture.
    /// Merges strings from base resources, embedded JSON, and contributors.
    /// Results are cached for performance.
    /// </summary>
    public async Task<IReadOnlyDictionary<string, string>> GetStringsAsync(
        Type resourceType, string cultureName)
    {
        var cultureCache = _cache.GetOrAdd(resourceType, _ => new());

        if (cultureCache.TryGetValue(cultureName, out var cached))
            return cached;

        var descriptor = GetDescriptor(resourceType);
        if (descriptor is null)
            return new Dictionary<string, string>();

        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 1. Load base resource strings first (fallback chain)
        foreach (var baseType in descriptor.BaseResourceTypes)
        {
            var baseStrings = await GetStringsAsync(baseType, cultureName);
            foreach (var kvp in baseStrings)
                merged[kvp.Key] = kvp.Value;
        }

        // 2. Load embedded JSON strings (override base)
        var embeddedStrings = _reader.ReadStrings(
            descriptor.Assembly,
            descriptor.ResourceBasePath,
            cultureName);

        if (embeddedStrings is not null)
        {
            foreach (var kvp in embeddedStrings)
                merged[kvp.Key] = kvp.Value;
        }

        // 3. Apply contributor strings (override embedded)
        foreach (var contributor in _contributors.Where(c => c.ResourceType == resourceType))
        {
            var contributedStrings = await contributor.GetStringsAsync(cultureName);
            if (contributedStrings is not null)
            {
                foreach (var kvp in contributedStrings)
                    merged[kvp.Key] = kvp.Value;
            }
        }

        var result = merged.AsReadOnly();
        cultureCache.TryAdd(cultureName, result);
        return result;
    }

    /// <summary>
    /// Gets localization strings synchronously (blocks on async).
    /// Prefer the async version when possible.
    /// </summary>
    public IReadOnlyDictionary<string, string> GetStrings(
        Type resourceType, string cultureName)
    {
        var cultureCache = _cache.GetOrAdd(resourceType, _ => new());

        if (cultureCache.TryGetValue(cultureName, out var cached))
            return cached;

        // For the synchronous code path with no contributors,
        // we can avoid Task overhead
        var descriptor = GetDescriptor(resourceType);
        if (descriptor is null)
            return new Dictionary<string, string>();

        var merged = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 1. Base resources
        foreach (var baseType in descriptor.BaseResourceTypes)
        {
            var baseStrings = GetStrings(baseType, cultureName);
            foreach (var kvp in baseStrings)
                merged[kvp.Key] = kvp.Value;
        }

        // 2. Embedded JSON
        var embeddedStrings = _reader.ReadStrings(
            descriptor.Assembly,
            descriptor.ResourceBasePath,
            cultureName);

        if (embeddedStrings is not null)
        {
            foreach (var kvp in embeddedStrings)
                merged[kvp.Key] = kvp.Value;
        }

        var result = merged.AsReadOnly();
        cultureCache.TryAdd(cultureName, result);
        return result;
    }

    /// <summary>
    /// Clears the entire localization cache. Useful after dynamic resource updates.
    /// </summary>
    public void ClearCache() => _cache.Clear();
}
