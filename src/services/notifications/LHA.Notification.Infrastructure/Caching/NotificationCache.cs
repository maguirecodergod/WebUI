using LHA.Notification.Application.Contracts;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure;

public sealed class NotificationCache
{
    private readonly HybridCache _cache;
    private readonly ILogger<NotificationCache> _logger;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

    public NotificationCache(HybridCache cache, ILogger<NotificationCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<NotificationDto?> GetAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        string key = $"notification:detail:{notificationId}";
        
        // Explicitly specifying type and providing a null-returning factory to simulate a "Get"
        return await _cache.GetOrCreateAsync<NotificationDto?>(
            key,
            _ => ValueTask.FromResult<NotificationDto?>(null),
            cancellationToken: cancellationToken);
    }

    public async Task SetAsync(string notificationId, NotificationDto dto, CancellationToken cancellationToken = default)
    {
        string key = $"notification:detail:{notificationId}";
        
        // HybridCache handles serialization internally.
        // We use SetAsync with options for expiration.
        await _cache.SetAsync(
            key, 
            dto, 
            new HybridCacheEntryOptions { Expiration = DefaultExpiry }, 
            cancellationToken: cancellationToken);
    }

    public async Task InvalidateAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        string key = $"notification:detail:{notificationId}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
