using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.Caching;

/// <summary>
/// Provides a cached user lookup service to resolve user display info by ID.
/// <para>
/// This is useful for resolving <c>CreatorId</c> / <c>LastModifierId</c> to
/// display names in UI without hitting the database on every request.
/// </para>
/// <para>
/// The factory delegate <see cref="UserLookupFactory"/> must be supplied
/// by the application layer (e.g., from Identity module's user repository).
/// </para>
/// </summary>
public sealed class CachedUserLookupService
{
    private readonly HybridCache _hybridCache;
    private readonly ILogger<CachedUserLookupService> _logger;

    /// <summary>
    /// Delegate that loads a user from the database.
    /// Must be registered by the application layer.
    /// </summary>
    private readonly Func<Guid, CancellationToken, Task<CachedUserItem?>>? _factory;

    private static readonly HybridCacheEntryOptions CacheOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(15),
        LocalCacheExpiration = TimeSpan.FromMinutes(5)
    };

    private static readonly string[] Tags = ["users"];

    public CachedUserLookupService(
        HybridCache hybridCache,
        Func<Guid, CancellationToken, Task<CachedUserItem?>>? factory = null,
        ILogger<CachedUserLookupService>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(hybridCache);

        _hybridCache = hybridCache;
        _factory = factory;
        _logger = logger ?? NullLogger<CachedUserLookupService>.Instance;
    }

    /// <summary>
    /// Gets a cached user by ID. If not in cache, calls the factory delegate
    /// to load from storage and caches the result.
    /// </summary>
    /// <returns>The cached user info, or <c>null</c> if not found.</returns>
    public async Task<CachedUserItem?> GetAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        if (_factory is null)
        {
            _logger.LogWarning(
                "CachedUserLookupService has no factory registered. " +
                "Register a Func<Guid, CancellationToken, Task<CachedUserItem?>> in DI.");
            return null;
        }

        var cacheKey = $"u:id:{userId}";
        return await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                _logger.LogDebug("Cache miss for user ID '{UserId}', loading from store.", userId);
                return await _factory(userId, ct);
            },
            CacheOptions,
            Tags,
            cancellationToken);
    }

    /// <summary>
    /// Gets multiple users by their IDs. Returns a dictionary of results.
    /// </summary>
    public async Task<Dictionary<Guid, CachedUserItem>> GetManyAsync(
        IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<Guid, CachedUserItem>();

        foreach (var userId in userIds.Distinct())
        {
            var user = await GetAsync(userId, cancellationToken);
            if (user is not null)
            {
                result[userId] = user;
            }
        }

        return result;
    }

    /// <summary>
    /// Resolves a user ID to a display name.
    /// Returns the user's full name, or falls back to "Unknown" if not found.
    /// </summary>
    public async Task<string> ResolveDisplayNameAsync(
        Guid? userId, CancellationToken cancellationToken = default)
    {
        if (!userId.HasValue) return "System";

        var user = await GetAsync(userId.Value, cancellationToken);
        return user?.GetFullName() ?? "Unknown";
    }

    /// <summary>
    /// Manually sets a user in the cache (e.g., after user creation or update).
    /// </summary>
    public async Task SetAsync(
        CachedUserItem userItem, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"u:id:{userItem.Id}";
        await _hybridCache.SetAsync(cacheKey, userItem, CacheOptions, Tags, cancellationToken);
    }

    /// <summary>
    /// Invalidates a specific user's cache entry.
    /// Call this when a user is updated or deleted.
    /// </summary>
    public async Task InvalidateAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invalidating cached user entry for ID '{UserId}'.", userId);
        await _hybridCache.RemoveAsync($"u:id:{userId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates all cached user entries.
    /// </summary>
    public async Task InvalidateAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invalidating all cached user entries.");
        await _hybridCache.RemoveByTagAsync("users", cancellationToken);
    }
}
