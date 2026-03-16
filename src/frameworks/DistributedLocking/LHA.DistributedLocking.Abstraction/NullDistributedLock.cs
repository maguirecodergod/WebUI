namespace LHA.DistributedLocking;

/// <summary>
/// A no-op implementation of <see cref="IDistributedLock"/> that always succeeds immediately.
/// Useful when distributed locking is not required or for testing.
/// </summary>
public sealed class NullDistributedLock : IDistributedLock
{
    /// <inheritdoc />
    public Task<IDistributedLockHandle?> TryAcquireAsync(
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IDistributedLockHandle?>(NullDistributedLockHandle.Instance);
    }
}
