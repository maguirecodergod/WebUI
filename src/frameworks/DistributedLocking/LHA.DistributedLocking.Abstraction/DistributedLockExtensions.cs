namespace LHA.DistributedLocking;

/// <summary>
/// Convenience extension methods for <see cref="IDistributedLock"/>.
/// </summary>
public static class DistributedLockExtensions
{
    /// <summary>
    /// Acquires a named distributed lock, throwing if the lock cannot be acquired
    /// within the specified <paramref name="timeout"/>.
    /// </summary>
    /// <exception cref="DistributedLockAcquisitionException">
    /// Thrown when the lock cannot be acquired within <paramref name="timeout"/>.
    /// </exception>
    public static async Task<IDistributedLockHandle> AcquireAsync(
        this IDistributedLock distributedLock,
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(distributedLock);

        var handle = await distributedLock.TryAcquireAsync(name, timeout, cancellationToken);

        return handle ?? throw new DistributedLockAcquisitionException(name, timeout);
    }
}
