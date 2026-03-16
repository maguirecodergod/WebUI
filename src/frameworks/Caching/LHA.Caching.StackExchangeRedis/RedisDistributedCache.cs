using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LHA.Caching.StackExchangeRedis;

/// <summary>
/// Redis-backed distributed cache that extends the standard <see cref="RedisCache"/>
/// with batch operations via <see cref="ICacheSupportsMultipleItems"/>.
/// Uses <see cref="IConnectionMultiplexer"/> directly for pipeline-friendly batch calls.
/// </summary>
public sealed class RedisDistributedCache : RedisCache, ICacheSupportsMultipleItems
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly RedisKey _instancePrefix;

    public RedisDistributedCache(
        IOptions<RedisCacheOptions> optionsAccessor,
        IConnectionMultiplexer connectionMultiplexer)
        : base(optionsAccessor)
    {
        _connectionMultiplexer = connectionMultiplexer;

        var instanceName = optionsAccessor.Value.InstanceName;
        _instancePrefix = string.IsNullOrEmpty(instanceName)
            ? default
            : (RedisKey)Encoding.UTF8.GetBytes(instanceName);
    }

    private IDatabase GetDatabase() => _connectionMultiplexer.GetDatabase();

    private RedisKey PrefixKey(string key) => _instancePrefix.Append(key);

    /// <inheritdoc />
    public byte[]?[] GetMany(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var db = GetDatabase();
        var keyArray = keys.ToArray();
        var tasks = new Task<RedisValue>[keyArray.Length];

        for (var i = 0; i < keyArray.Length; i++)
        {
            tasks[i] = db.HashGetAsync(PrefixKey(keyArray[i]), "data");
        }

        Task.WaitAll(tasks);

        var results = new byte[]?[keyArray.Length];
        for (var i = 0; i < keyArray.Length; i++)
        {
            results[i] = tasks[i].Result.HasValue ? (byte[]?)tasks[i].Result : null;
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<byte[]?[]> GetManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();

        var db = GetDatabase();
        var keyArray = keys.ToArray();
        var tasks = new Task<RedisValue>[keyArray.Length];

        for (var i = 0; i < keyArray.Length; i++)
        {
            tasks[i] = db.HashGetAsync(PrefixKey(keyArray[i]), "data");
        }

        await Task.WhenAll(tasks);

        var results = new byte[]?[keyArray.Length];
        for (var i = 0; i < keyArray.Length; i++)
        {
            results[i] = tasks[i].Result.HasValue ? (byte[]?)tasks[i].Result : null;
        }

        return results;
    }

    /// <inheritdoc />
    public void SetMany(
        IEnumerable<KeyValuePair<string, byte[]>> items,
        DistributedCacheEntryOptions options)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(options);

        // Delegate to the base class per-item for correct expiration handling.
        foreach (var item in items)
        {
            Set(item.Key, item.Value, options);
        }
    }

    /// <inheritdoc />
    public async Task SetManyAsync(
        IEnumerable<KeyValuePair<string, byte[]>> items,
        DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(options);

        // Delegate to the base class per-item for correct expiration handling.
        foreach (var item in items)
        {
            await SetAsync(item.Key, item.Value, options, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void RefreshMany(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        foreach (var key in keys)
        {
            Refresh(key);
        }
    }

    /// <inheritdoc />
    public async Task RefreshManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        foreach (var key in keys)
        {
            await RefreshAsync(key, cancellationToken);
        }
    }

    /// <inheritdoc />
    public void RemoveMany(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var db = GetDatabase();
        var redisKeys = keys.Select(k => PrefixKey(k)).ToArray();
        Task.WaitAll(redisKeys.Select(k => db.KeyDeleteAsync(k)).ToArray<Task>());
    }

    /// <inheritdoc />
    public async Task RemoveManyAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();

        var db = GetDatabase();
        var redisKeys = keys.Select(k => PrefixKey(k)).ToArray();
        await Task.WhenAll(redisKeys.Select(k => db.KeyDeleteAsync(k)));
    }
}
