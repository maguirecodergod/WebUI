using LHA.DistributedLocking;
using MedallionLocking = Medallion.Threading;

namespace LHA.DistributedLocking.Medallion;

/// <summary>
/// Wraps a Medallion <see cref="MedallionLocking.IDistributedSynchronizationHandle"/>
/// as an <see cref="IDistributedLockHandle"/>.
/// </summary>
internal sealed class MedallionDistributedLockHandle : IDistributedLockHandle
{
    private readonly MedallionLocking.IDistributedSynchronizationHandle _inner;

    public MedallionDistributedLockHandle(MedallionLocking.IDistributedSynchronizationHandle inner)
    {
        _inner = inner;
    }

    /// <inheritdoc />
    public CancellationToken HandleLostToken => _inner.HandleLostToken;

    /// <inheritdoc />
    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
