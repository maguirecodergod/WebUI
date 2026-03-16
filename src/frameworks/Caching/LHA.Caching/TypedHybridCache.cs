using LHA.MultiTenancy;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.Caching;

/// <summary>
/// Type-safe hybrid cache with a <see langword="string"/> key.
/// </summary>
public sealed class TypedHybridCache<TCacheItem> :
    TypedHybridCache<TCacheItem, string>,
    ITypedHybridCache<TCacheItem>
    where TCacheItem : class
{
    public TypedHybridCache(
        HybridCache hybridCache,
        ICacheKeyNormalizer keyNormalizer,
        IOptions<CachingOptions> options,
        ILoggerFactory? loggerFactory = null)
        : base(hybridCache, keyNormalizer, options, loggerFactory)
    {
    }
}

/// <summary>
/// Type-safe hybrid cache implementation that wraps the .NET <see cref="HybridCache"/>
/// with tenant-aware key normalization and error-hiding behaviour.
/// </summary>
/// <typeparam name="TCacheItem">The cache item type (must be a reference type).</typeparam>
/// <typeparam name="TCacheKey">The cache key type.</typeparam>
public class TypedHybridCache<TCacheItem, TCacheKey> : ITypedHybridCache<TCacheItem, TCacheKey>
    where TCacheItem : class
    where TCacheKey : notnull
{
    private readonly HybridCache _hybridCache;
    private readonly ICacheKeyNormalizer _keyNormalizer;
    private readonly CachingOptions _options;
    private readonly ILogger _logger;
    private readonly string _cacheName;
    private readonly bool _ignoreMultiTenancy;

    public TypedHybridCache(
        HybridCache hybridCache,
        ICacheKeyNormalizer keyNormalizer,
        IOptions<CachingOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _hybridCache = hybridCache;
        _keyNormalizer = keyNormalizer;
        _options = options.Value;
        _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;
        _cacheName = CacheNameAttribute.GetCacheName(typeof(TCacheItem));
        _ignoreMultiTenancy = typeof(TCacheItem).IsDefined(typeof(IgnoreMultiTenancyAttribute), true);
    }

    /// <inheritdoc />
    public virtual async Task<TCacheItem?> GetOrCreateAsync(
        TCacheKey key,
        Func<Task<TCacheItem>> factory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _hybridCache.GetOrCreateAsync(
                key: NormalizeKey(key),
                factory: async ct => await factory(),
                options: options,
                tags: tags,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            return null;
        }
    }

    /// <inheritdoc />
    public virtual async Task SetAsync(
        TCacheKey key,
        TCacheItem value,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hybridCache.SetAsync(
                key: NormalizeKey(key),
                value: value,
                options: options,
                tags: tags,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            // Swallowed and logged.
        }
    }

    /// <inheritdoc />
    public virtual async Task RemoveAsync(TCacheKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hybridCache.RemoveAsync(NormalizeKey(key), cancellationToken);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            // Swallowed and logged.
        }
    }

    /// <summary>
    /// Normalizes a typed key to its full string representation.
    /// </summary>
    protected string NormalizeKey(TCacheKey key) =>
        _keyNormalizer.NormalizeKey(_cacheName, key.ToString()!, _ignoreMultiTenancy);

    private bool ShouldHideError(Exception ex)
    {
        if (!_options.HideErrors) return false;
        _logger.LogWarning(ex, "Hybrid cache error for cache '{CacheName}' was hidden.", _cacheName);
        return true;
    }
}
