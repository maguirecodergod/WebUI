using Microsoft.Extensions.Caching.Distributed;

namespace LHA.Caching;

/// <summary>
/// Type-safe distributed cache with a <see langword="string"/> key.
/// </summary>
/// <typeparam name="TCacheItem">The cache item type.</typeparam>
public interface ITypedDistributedCache<TCacheItem> : ITypedDistributedCache<TCacheItem, string>
    where TCacheItem : class;

/// <summary>
/// Type-safe distributed cache with a typed key.
/// Wraps <see cref="IDistributedCache"/> and adds automatic serialization,
/// key normalization (cache name + tenant prefix), and batch operations.
/// </summary>
/// <typeparam name="TCacheItem">The cache item type.</typeparam>
/// <typeparam name="TCacheKey">The cache key type.</typeparam>
public interface ITypedDistributedCache<TCacheItem, TCacheKey>
    where TCacheItem : class
    where TCacheKey : notnull
{
    /// <summary>
    /// Gets a cache item by key, returning <see langword="null"/> if not found.
    /// </summary>
    Task<TCacheItem?> GetAsync(TCacheKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or adds a cache item. If absent, <paramref name="factory"/> is called to produce the value
    /// which is then stored in the cache before being returned.
    /// </summary>
    Task<TCacheItem?> GetOrAddAsync(
        TCacheKey key,
        Func<Task<TCacheItem>> factory,
        Func<DistributedCacheEntryOptions>? optionsFactory = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a cache item.
    /// </summary>
    Task SetAsync(
        TCacheKey key,
        TCacheItem value,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple cache items by their keys.
    /// </summary>
    Task<KeyValuePair<TCacheKey, TCacheItem?>[]> GetManyAsync(
        IEnumerable<TCacheKey> keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets multiple cache items in a single batch.
    /// </summary>
    Task SetManyAsync(
        IEnumerable<KeyValuePair<TCacheKey, TCacheItem>> items,
        DistributedCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the sliding expiration timeout of a cache item.
    /// </summary>
    Task RefreshAsync(TCacheKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cache item.
    /// </summary>
    Task RemoveAsync(TCacheKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes multiple cache items.
    /// </summary>
    Task RemoveManyAsync(IEnumerable<TCacheKey> keys, CancellationToken cancellationToken = default);
}
