using Microsoft.Extensions.Caching.Hybrid;

namespace LHA.Caching;

/// <summary>
/// Type-safe hybrid cache (L1 in-memory + L2 distributed) with a <see langword="string"/> key.
/// </summary>
/// <typeparam name="TCacheItem">The cache item type.</typeparam>
public interface ITypedHybridCache<TCacheItem> : ITypedHybridCache<TCacheItem, string>
    where TCacheItem : class;

/// <summary>
/// Type-safe hybrid cache (L1 in-memory + L2 distributed) with a typed key.
/// Wraps <see cref="HybridCache"/> and adds automatic key normalization
/// (cache name + tenant prefix).
/// </summary>
/// <typeparam name="TCacheItem">The cache item type.</typeparam>
/// <typeparam name="TCacheKey">The cache key type.</typeparam>
public interface ITypedHybridCache<TCacheItem, TCacheKey>
    where TCacheItem : class
    where TCacheKey : notnull
{
    /// <summary>
    /// Gets or creates a cache item. If absent, <paramref name="factory"/> is called to
    /// produce the value which is then stored in both L1 and L2 caches.
    /// </summary>
    Task<TCacheItem?> GetOrCreateAsync(
        TCacheKey key,
        Func<Task<TCacheItem>> factory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a cache item in both L1 and L2.
    /// </summary>
    Task SetAsync(
        TCacheKey key,
        TCacheItem value,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a cache item from both L1 and L2.
    /// </summary>
    Task RemoveAsync(TCacheKey key, CancellationToken cancellationToken = default);
}
