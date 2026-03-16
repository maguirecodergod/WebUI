using LHA.Core.Threading;

namespace LHA.DistributedLocking;

/// <summary>
/// In-process distributed lock backed by <see cref="KeyedLock"/>.
/// Suitable for single-instance deployments or as a development fallback.
/// </summary>
public sealed class LocalDistributedLock : IDistributedLock
{
    private readonly IDistributedLockKeyNormalizer _keyNormalizer;

    public LocalDistributedLock(IDistributedLockKeyNormalizer keyNormalizer)
    {
        ArgumentNullException.ThrowIfNull(keyNormalizer);
        _keyNormalizer = keyNormalizer;
    }

    /// <inheritdoc />
    public async Task<IDistributedLockHandle?> TryAcquireAsync(
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        var key = _keyNormalizer.NormalizeKey(name);
        var releaser = await KeyedLock.TryLockAsync(key, timeout, cancellationToken);

        return releaser is null ? null : new LocalDistributedLockHandle(releaser);
    }
}
