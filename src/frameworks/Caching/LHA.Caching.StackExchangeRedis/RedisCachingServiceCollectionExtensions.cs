using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace LHA.Caching.StackExchangeRedis;

/// <summary>
/// Extension methods for registering Redis-backed LHA caching.
/// </summary>
public static class RedisCachingServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the default in-memory distributed cache with a Redis-backed implementation
    /// and registers <see cref="ICacheSupportsMultipleItems"/> for batch operations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureRedis">Callback to configure <see cref="RedisCacheOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHAStackExchangeRedisCache(
        this IServiceCollection services,
        Action<RedisCacheOptions> configureRedis)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureRedis);

        services.Configure(configureRedis);

        // Register the IConnectionMultiplexer singleton from options if not already registered.
        services.TryAddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RedisCacheOptions>>().Value;
            var configuration = options.Configuration ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(configuration);
        });

        // Replace the default IDistributedCache with the Redis implementation
        // that also supports ICacheSupportsMultipleItems.
        services.RemoveAll<IDistributedCache>();
        services.AddSingleton<RedisDistributedCache>();
        services.AddSingleton<IDistributedCache>(sp => sp.GetRequiredService<RedisDistributedCache>());
        services.AddSingleton<ICacheSupportsMultipleItems>(sp => sp.GetRequiredService<RedisDistributedCache>());

        return services;
    }
}
