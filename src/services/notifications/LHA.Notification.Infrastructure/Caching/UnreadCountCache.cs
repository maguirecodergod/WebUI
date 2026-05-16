using StackExchange.Redis;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Infrastructure;

public class UnreadCountCache : IUnreadCountCache
{
    private readonly IDatabase _redis;
    private readonly INotificationRepository _notificationRepository;
    private readonly TimeSpan _ttl = TimeSpan.FromHours(NotificationConstants.UnreadCountCacheTtlHours);

    public UnreadCountCache(IConnectionMultiplexer redis, INotificationRepository notificationRepository)
    {
        _redis = redis.GetDatabase();
        _notificationRepository = notificationRepository;
    }

    public async Task<int> GetUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        var key = string.Format(CacheKeys.UnreadCountKeyFormat, tenantId, recipientId);
        var cached = await _redis.StringGetAsync(key);

        if (cached.HasValue)
        {
            return (int)cached;
        }

        var count = await _notificationRepository.GetUnreadCountByRecipientAsync(recipientId, tenantId, cancellationToken);
        await _redis.StringSetAsync(key, count, _ttl);
        return count;
    }

    public async Task SetUnreadCountAsync(Guid tenantId, Guid recipientId, int count, CancellationToken cancellationToken = default)
    {
        var key = string.Format(CacheKeys.UnreadCountKeyFormat, tenantId, recipientId);
        await _redis.StringSetAsync(key, count, _ttl);
    }

    public async Task IncrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        var key = string.Format(CacheKeys.UnreadCountKeyFormat, tenantId, recipientId);
        await _redis.StringIncrementAsync(key);
    }

    public async Task DecrementUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        var key = string.Format(CacheKeys.UnreadCountKeyFormat, tenantId, recipientId);
        await _redis.StringDecrementAsync(key);
    }

    public async Task ClearUnreadCountAsync(Guid tenantId, Guid recipientId, CancellationToken cancellationToken = default)
    {
        var key = string.Format(CacheKeys.UnreadCountKeyFormat, tenantId, recipientId);
        await _redis.KeyDeleteAsync(key);
    }
}
