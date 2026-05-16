using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure;

public sealed class DeviceTokenCache
{
    private readonly HybridCache _cache;
    private readonly ILogger<DeviceTokenCache> _logger;
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(1);

    public DeviceTokenCache(HybridCache cache, ILogger<DeviceTokenCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> GetDeviceIdByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        string key = $"device:token:{tokenHash}";
        return await _cache.GetOrCreateAsync<string?>(
            key, 
            _ => ValueTask.FromResult<string?>(null), 
            cancellationToken: cancellationToken);
    }

    public async Task SetAsync(string tokenHash, string deviceId, CancellationToken cancellationToken = default)
    {
        string key = $"device:token:{tokenHash}";
        await _cache.SetAsync(
            key, 
            deviceId, 
            new HybridCacheEntryOptions { Expiration = DefaultExpiry }, 
            cancellationToken: cancellationToken);
    }

    public async Task InvalidateAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        string key = $"device:token:{tokenHash}";
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
