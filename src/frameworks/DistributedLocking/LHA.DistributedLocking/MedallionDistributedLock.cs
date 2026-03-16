using LHA.DistributedLocking;
using Medallion.Threading;
using MedallionLocking = Medallion.Threading;

namespace LHA.DistributedLocking.Medallion;

/// <summary>
/// Adapts a Medallion <see cref="MedallionLocking.IDistributedLockProvider"/>
/// to the LHA <see cref="IDistributedLock"/> abstraction.
/// </summary>
public sealed class MedallionDistributedLock : IDistributedLock
{
    private readonly MedallionLocking.IDistributedLockProvider _provider;
    private readonly IDistributedLockKeyNormalizer _keyNormalizer;

    public MedallionDistributedLock(
        MedallionLocking.IDistributedLockProvider provider,
        IDistributedLockKeyNormalizer keyNormalizer)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(keyNormalizer);
        _provider = provider;
        _keyNormalizer = keyNormalizer;
    }

    /// <inheritdoc />
    public async Task<IDistributedLockHandle?> TryAcquireAsync(
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        var key = _keyNormalizer.NormalizeKey(name);
        var handle = await _provider.TryAcquireLockAsync(key, timeout, cancellationToken);

        return handle is null ? null : new MedallionDistributedLockHandle(handle);
    }
}
