using Microsoft.Extensions.Caching.Distributed;

namespace LHA.Caching;

/// <summary>
/// Extension of <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>
/// that supports batch Get, Set, Refresh, and Remove operations.
/// Implemented by providers such as Redis that natively support pipelining.
/// </summary>
public interface ICacheSupportsMultipleItems
{
    /// <summary>Gets multiple cache entries by their normalized keys.</summary>
    byte[]?[] GetMany(IEnumerable<string> keys);

    /// <summary>Gets multiple cache entries by their normalized keys.</summary>
    Task<byte[]?[]> GetManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>Sets multiple cache entries in a single batch.</summary>
    void SetMany(IEnumerable<KeyValuePair<string, byte[]>> items, DistributedCacheEntryOptions options);

    /// <summary>Sets multiple cache entries in a single batch.</summary>
    Task SetManyAsync(IEnumerable<KeyValuePair<string, byte[]>> items, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

    /// <summary>Refreshes the sliding expiration of multiple cache entries.</summary>
    void RefreshMany(IEnumerable<string> keys);

    /// <summary>Refreshes the sliding expiration of multiple cache entries.</summary>
    Task RefreshManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>Removes multiple cache entries.</summary>
    void RemoveMany(IEnumerable<string> keys);

    /// <summary>Removes multiple cache entries.</summary>
    Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
