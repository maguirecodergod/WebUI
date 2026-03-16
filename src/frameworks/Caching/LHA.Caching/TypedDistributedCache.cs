using LHA.MultiTenancy;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace LHA.Caching;

/// <summary>
/// Type-safe distributed cache with a <see langword="string"/> key.
/// Delegates to <see cref="TypedDistributedCache{TCacheItem, TCacheKey}"/>.
/// </summary>
public sealed class TypedDistributedCache<TCacheItem> :
    TypedDistributedCache<TCacheItem, string>,
    ITypedDistributedCache<TCacheItem>
    where TCacheItem : class
{
    public TypedDistributedCache(
        IDistributedCache cache,
        IDistributedCacheSerializer serializer,
        ICacheKeyNormalizer keyNormalizer,
        IOptions<CachingOptions> options,
        ILoggerFactory? loggerFactory = null)
        : base(cache, serializer, keyNormalizer, options, loggerFactory)
    {
    }
}

/// <summary>
/// Type-safe distributed cache implementation that wraps <see cref="IDistributedCache"/>
/// with automatic JSON serialization, tenant-aware key normalization, and batch support.
/// </summary>
/// <typeparam name="TCacheItem">The cache item type (must be a reference type).</typeparam>
/// <typeparam name="TCacheKey">The cache key type.</typeparam>
public class TypedDistributedCache<TCacheItem, TCacheKey> : ITypedDistributedCache<TCacheItem, TCacheKey>
    where TCacheItem : class
    where TCacheKey : notnull
{
    private readonly IDistributedCache _cache;
    private readonly IDistributedCacheSerializer _serializer;
    private readonly ICacheKeyNormalizer _keyNormalizer;
    private readonly CachingOptions _options;
    private readonly ILogger _logger;
    private readonly string _cacheName;
    private readonly bool _ignoreMultiTenancy;
    private readonly DistributedCacheEntryOptions _defaultEntryOptions;
    private readonly SemaphoreSlim _syncSemaphore = new(1, 1);

    public TypedDistributedCache(
        IDistributedCache cache,
        IDistributedCacheSerializer serializer,
        ICacheKeyNormalizer keyNormalizer,
        IOptions<CachingOptions> options,
        ILoggerFactory? loggerFactory = null)
    {
        _cache = cache;
        _serializer = serializer;
        _keyNormalizer = keyNormalizer;
        _options = options.Value;
        _logger = loggerFactory?.CreateLogger(GetType()) ?? NullLogger.Instance;
        _cacheName = CacheNameAttribute.GetCacheName(typeof(TCacheItem));
        _ignoreMultiTenancy = typeof(TCacheItem).IsDefined(typeof(IgnoreMultiTenancyAttribute), true);
        _defaultEntryOptions = ResolveDefaultEntryOptions();
    }

    /// <inheritdoc />
    public virtual async Task<TCacheItem?> GetAsync(TCacheKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await _cache.GetAsync(NormalizeKey(key), cancellationToken);
            return bytes is null ? null : _serializer.Deserialize<TCacheItem>(bytes);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            return null;
        }
    }

    /// <inheritdoc />
    public virtual async Task<TCacheItem?> GetOrAddAsync(
        TCacheKey key,
        Func<Task<TCacheItem>> factory,
        Func<DistributedCacheEntryOptions>? optionsFactory = null,
        CancellationToken cancellationToken = default)
    {
        var value = await GetAsync(key, cancellationToken);
        if (value is not null)
        {
            return value;
        }

        await _syncSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring semaphore.
            value = await GetAsync(key, cancellationToken);
            if (value is not null)
            {
                return value;
            }

            value = await factory();
            await SetAsync(key, value, optionsFactory?.Invoke(), cancellationToken);
            return value;
        }
        finally
        {
            _syncSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public virtual async Task SetAsync(
        TCacheKey key,
        TCacheItem value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = _serializer.Serialize(value);
            await _cache.SetAsync(NormalizeKey(key), bytes, options ?? _defaultEntryOptions, cancellationToken);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            // Swallowed and logged.
        }
    }

    /// <inheritdoc />
    public virtual async Task<KeyValuePair<TCacheKey, TCacheItem?>[]> GetManyAsync(
        IEnumerable<TCacheKey> keys,
        CancellationToken cancellationToken = default)
    {
        var keyArray = keys.ToArray();

        if (_cache is ICacheSupportsMultipleItems batchCache)
        {
            try
            {
                var normalizedKeys = keyArray.Select(NormalizeKey);
                var bytesArray = await batchCache.GetManyAsync(normalizedKeys, cancellationToken);

                return keyArray.Zip(bytesArray, (k, bytes) =>
                    new KeyValuePair<TCacheKey, TCacheItem?>(k, bytes is null ? null : _serializer.Deserialize<TCacheItem>(bytes))
                ).ToArray();
            }
            catch (Exception ex) when (ShouldHideError(ex))
            {
                return keyArray.Select(k => new KeyValuePair<TCacheKey, TCacheItem?>(k, null)).ToArray();
            }
        }

        // Fallback: sequential gets.
        var results = new KeyValuePair<TCacheKey, TCacheItem?>[keyArray.Length];
        for (var i = 0; i < keyArray.Length; i++)
        {
            results[i] = new KeyValuePair<TCacheKey, TCacheItem?>(
                keyArray[i],
                await GetAsync(keyArray[i], cancellationToken));
        }
        return results;
    }

    /// <inheritdoc />
    public virtual async Task SetManyAsync(
        IEnumerable<KeyValuePair<TCacheKey, TCacheItem>> items,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var entryOptions = options ?? _defaultEntryOptions;

        if (_cache is ICacheSupportsMultipleItems batchCache)
        {
            try
            {
                var raw = items.Select(i =>
                    new KeyValuePair<string, byte[]>(NormalizeKey(i.Key), _serializer.Serialize(i.Value)));
                await batchCache.SetManyAsync(raw, entryOptions, cancellationToken);
                return;
            }
            catch (Exception ex) when (ShouldHideError(ex))
            {
                return;
            }
        }

        // Fallback: sequential sets.
        foreach (var item in items)
        {
            await SetAsync(item.Key, item.Value, entryOptions, cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual async Task RefreshAsync(TCacheKey key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RefreshAsync(NormalizeKey(key), cancellationToken);
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
            await _cache.RemoveAsync(NormalizeKey(key), cancellationToken);
        }
        catch (Exception ex) when (ShouldHideError(ex))
        {
            // Swallowed and logged.
        }
    }

    /// <inheritdoc />
    public virtual async Task RemoveManyAsync(IEnumerable<TCacheKey> keys, CancellationToken cancellationToken = default)
    {
        if (_cache is ICacheSupportsMultipleItems batchCache)
        {
            try
            {
                await batchCache.RemoveManyAsync(keys.Select(NormalizeKey), cancellationToken);
                return;
            }
            catch (Exception ex) when (ShouldHideError(ex))
            {
                return;
            }
        }

        // Fallback: sequential removes.
        foreach (var key in keys)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }

    /// <summary>
    /// Normalizes a typed key to its full string representation.
    /// </summary>
    protected string NormalizeKey(TCacheKey key) =>
        _keyNormalizer.NormalizeKey(_cacheName, key.ToString()!, _ignoreMultiTenancy);

    private DistributedCacheEntryOptions ResolveDefaultEntryOptions()
    {
        foreach (var configurator in _options.CacheConfigurators)
        {
            var options = configurator(_cacheName);
            if (options is not null) return options;
        }
        return _options.GlobalCacheEntryOptions;
    }

    private bool ShouldHideError(Exception ex)
    {
        if (!_options.HideErrors) return false;
        _logger.LogWarning(ex, "Distributed cache error for cache '{CacheName}' was hidden.", _cacheName);
        return true;
    }
}
