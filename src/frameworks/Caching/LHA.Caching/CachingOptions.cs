using Microsoft.Extensions.Caching.Distributed;

namespace LHA.Caching;

/// <summary>
/// Options for the LHA distributed caching infrastructure.
/// </summary>
public sealed class CachingOptions
{
    /// <summary>
    /// When <see langword="true"/>, cache exceptions are swallowed and logged
    /// instead of propagating to callers. Default: <see langword="true"/>.
    /// </summary>
    public bool HideErrors { get; set; } = true;

    /// <summary>
    /// Global key prefix prepended to all cache keys (e.g. an application name).
    /// Default: empty string.
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Default <see cref="DistributedCacheEntryOptions"/> applied when a caller does not
    /// specify options. Default: 20-minute sliding expiration.
    /// </summary>
    public DistributedCacheEntryOptions GlobalCacheEntryOptions { get; set; } = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(20)
    };

    /// <summary>
    /// Per-cache-name configurators. Each function receives the cache name and returns
    /// custom <see cref="DistributedCacheEntryOptions"/> (or <see langword="null"/> to skip).
    /// The first non-null result wins.
    /// </summary>
    public List<Func<string, DistributedCacheEntryOptions?>> CacheConfigurators { get; } = [];

    /// <summary>
    /// Configures entry options for a specific cache item type.
    /// </summary>
    public void ConfigureCache<TCacheItem>(DistributedCacheEntryOptions options) =>
        ConfigureCache(CacheNameAttribute.GetCacheName<TCacheItem>(), options);

    /// <summary>
    /// Configures entry options for a specific cache name.
    /// </summary>
    public void ConfigureCache(string cacheName, DistributedCacheEntryOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheName);
        CacheConfigurators.Add(name => string.Equals(name, cacheName, StringComparison.Ordinal) ? options : null);
    }
}
