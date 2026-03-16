namespace LHA.Caching;

/// <summary>
/// Normalizes a raw cache key into a fully-qualified key that includes
/// the cache name, key prefix, and tenant identifier (when applicable).
/// </summary>
public interface ICacheKeyNormalizer
{
    /// <summary>
    /// Normalizes the given <paramref name="key"/> for the specified <paramref name="cacheName"/>.
    /// </summary>
    /// <param name="cacheName">The logical cache name (resolved via <see cref="CacheNameAttribute"/>).</param>
    /// <param name="key">The raw cache key.</param>
    /// <param name="ignoreMultiTenancy">
    /// When <see langword="true"/>, the tenant prefix is omitted even if a current tenant exists.
    /// </param>
    string NormalizeKey(string cacheName, string key, bool ignoreMultiTenancy = false);
}
