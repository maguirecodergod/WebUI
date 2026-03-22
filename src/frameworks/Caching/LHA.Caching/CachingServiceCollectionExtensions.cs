using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Caching;

/// <summary>
/// Extension methods for registering LHA caching services.
/// </summary>
public static class CachingServiceCollectionExtensions
{
    /// <summary>
    /// Registers LHA caching infrastructure including in-memory caching,
    /// distributed memory cache (default L2), hybrid cache, typed cache wrappers,
    /// key normalizer, and JSON serializer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureCaching">Optional callback to configure <see cref="CachingOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHACaching(
        this IServiceCollection services,
        Action<CachingOptions>? configureCaching = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Options
        if (configureCaching is not null)
        {
            services.Configure(configureCaching);
        }

        // Memory + distributed memory (default L2, can be replaced by Redis etc.)
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        // Hybrid cache (L1 + L2)
#pragma warning disable EXTEXP0018 // HybridCache is experimental in some previews
        services.AddHybridCache();
#pragma warning restore EXTEXP0018

        // Infrastructure
        services.TryAddSingleton<ICacheKeyNormalizer, CacheKeyNormalizer>();
        services.TryAddSingleton<IDistributedCacheSerializer, JsonDistributedCacheSerializer>();

        // Typed caches (open generic registrations)
        services.TryAddSingleton(typeof(ITypedDistributedCache<>), typeof(TypedDistributedCache<>));
        services.TryAddSingleton(typeof(ITypedDistributedCache<,>), typeof(TypedDistributedCache<,>));
        services.TryAddSingleton(typeof(ITypedHybridCache<>), typeof(TypedHybridCache<>));
        services.TryAddSingleton(typeof(ITypedHybridCache<,>), typeof(TypedHybridCache<,>));

        // User/Tenant lookup caches
        services.TryAddSingleton<CachedUserLookupService>();

        return services;
    }
}
