using LHA.MultiTenancy;
using Microsoft.Extensions.Options;

namespace LHA.Caching;

/// <summary>
/// Default key normalizer that produces keys in the format:
/// <c>t:{tenantId},c:{cacheName},k:{prefix}{rawKey}</c>.
/// The tenant segment is omitted for host-level or tenant-agnostic caches.
/// </summary>
public sealed class CacheKeyNormalizer : ICacheKeyNormalizer
{
    private readonly ICurrentTenant _currentTenant;
    private readonly CachingOptions _options;

    public CacheKeyNormalizer(
        ICurrentTenant currentTenant,
        IOptions<CachingOptions> options)
    {
        _currentTenant = currentTenant;
        _options = options.Value;
    }

    /// <inheritdoc />
    public string NormalizeKey(string cacheName, string key, bool ignoreMultiTenancy = false)
    {
        var normalizedKey = $"c:{cacheName},k:{_options.KeyPrefix}{key}";

        if (!ignoreMultiTenancy && _currentTenant.Id.HasValue)
        {
            normalizedKey = $"t:{_currentTenant.Id.Value},{normalizedKey}";
        }

        return normalizedKey;
    }
}
