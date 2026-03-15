namespace LHA.Security.ReplayProtection;

public interface INonceStore
{
    Task<bool> TryAddAsync(string nonce, TimeSpan expiry);
}

public class ReplayProtectionService
{
    private readonly INonceStore _nonceStore;
    private readonly int _maxRequestAgeSeconds;

    public ReplayProtectionService(INonceStore nonceStore, int maxRequestAgeSeconds = 30)
    {
        _nonceStore = nonceStore;
        _maxRequestAgeSeconds = maxRequestAgeSeconds;
    }

    public async Task<bool> IsValidAsync(string nonce, string timestampStr)
    {
        if (!long.TryParse(timestampStr, out var timestamp)) return false;

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var now = DateTimeOffset.UtcNow;

        // Check if timestamp is too old or in the future
        if (Math.Abs((now - requestTime).TotalSeconds) > _maxRequestAgeSeconds)
        {
            return false;
        }

        // Try to add nonce to store (fails if exists)
        return await _nonceStore.TryAddAsync(nonce, TimeSpan.FromSeconds(_maxRequestAgeSeconds * 2));
    }
}
