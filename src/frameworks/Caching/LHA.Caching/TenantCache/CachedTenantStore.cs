using LHA.MultiTenancy;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.Caching;

/// <summary>
/// Decorator that wraps an <see cref="ITenantStore"/> and caches
/// <see cref="TenantConfiguration"/> lookups in <see cref="HybridCache"/>
/// (L1 in-memory + L2 distributed).
/// <para>
/// Cache keys are normalized with a <c>t:</c> prefix. Entries are tagged
/// with <c>"tenants"</c> so they can be bulk-invalidated when tenants change.
/// </para>
/// </summary>
public sealed class CachedTenantStore : ITenantStore
{
    private readonly ITenantStore _inner;
    private readonly HybridCache _hybridCache;
    private readonly ILogger<CachedTenantStore> _logger;

    /// <summary>
    /// Default cache duration for tenant lookups.
    /// Tenant data changes rarely so a longer TTL is acceptable.
    /// </summary>
    private static readonly HybridCacheEntryOptions CacheOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(30),
        LocalCacheExpiration = TimeSpan.FromMinutes(10)
    };

    private static readonly string[] Tags = ["tenants"];

    public CachedTenantStore(
        ITenantStore inner,
        HybridCache hybridCache,
        ILogger<CachedTenantStore>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(hybridCache);

        _inner = inner;
        _hybridCache = hybridCache;
        _logger = logger ?? NullLogger<CachedTenantStore>.Instance;
    }

    /// <inheritdoc />
    public async Task<TenantConfiguration?> FindAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"t:id:{id}";
        return await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                _logger.LogDebug("Cache miss for tenant ID '{TenantId}', loading from store.", id);
                return await _inner.FindAsync(id, ct);
            },
            CacheOptions,
            Tags,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TenantConfiguration?> FindByNameAsync(
        string normalizedName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"t:name:{normalizedName}";
        return await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                _logger.LogDebug("Cache miss for tenant name '{TenantName}', loading from store.", normalizedName);
                return await _inner.FindByNameAsync(normalizedName, ct);
            },
            CacheOptions,
            Tags,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantConfiguration>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        const string cacheKey = "t:list:all";
        var result = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                _logger.LogDebug("Cache miss for tenant list, loading from store.");
                var list = await _inner.GetListAsync(ct);
                return list.ToList(); // HybridCache needs a concrete serializable type
            },
            CacheOptions,
            Tags,
            cancellationToken);

        return result ?? [];
    }

    /// <summary>
    /// Invalidates all cached tenant entries.
    /// Call this when a tenant is created, updated, or deleted.
    /// </summary>
    public async Task InvalidateAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invalidating all cached tenant entries.");
        await _hybridCache.RemoveByTagAsync("tenants", cancellationToken);
    }

    /// <summary>
    /// Invalidates a specific tenant's cached entry.
    /// </summary>
    public async Task InvalidateAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invalidating cached tenant entry for ID '{TenantId}'.", tenantId);
        await _hybridCache.RemoveAsync($"t:id:{tenantId}", cancellationToken);
        // Also invalidate the list since it may be stale
        await _hybridCache.RemoveByTagAsync("tenants", cancellationToken);
    }
}
