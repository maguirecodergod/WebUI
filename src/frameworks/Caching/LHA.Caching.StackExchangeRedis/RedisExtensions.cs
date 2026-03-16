using StackExchange.Redis;

namespace LHA.Caching.StackExchangeRedis;

/// <summary>
/// Extension methods for pipelining Redis hash operations across multiple keys.
/// </summary>
public static class RedisExtensions
{
    /// <summary>
    /// Pipelines <c>HMGET</c> calls for each key and returns the results in order.
    /// </summary>
    public static RedisValue[][] HashMemberGetMany(
        this IDatabase database,
        RedisKey[] keys,
        RedisValue[] fields)
    {
        var tasks = new Task<RedisValue[]>[keys.Length];

        for (var i = 0; i < keys.Length; i++)
        {
            tasks[i] = database.HashGetAsync(keys[i], fields);
        }

        Task.WaitAll(tasks);

        var results = new RedisValue[keys.Length][];
        for (var i = 0; i < keys.Length; i++)
        {
            results[i] = tasks[i].Result;
        }

        return results;
    }

    /// <summary>
    /// Async version of <see cref="HashMemberGetMany"/>.
    /// </summary>
    public static async Task<RedisValue[][]> HashMemberGetManyAsync(
        this IDatabase database,
        RedisKey[] keys,
        RedisValue[] fields)
    {
        var tasks = new Task<RedisValue[]>[keys.Length];

        for (var i = 0; i < keys.Length; i++)
        {
            tasks[i] = database.HashGetAsync(keys[i], fields);
        }

        return await Task.WhenAll(tasks);
    }
}
