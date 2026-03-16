namespace LHA.DistributedLocking;

/// <summary>
/// Provides distributed locking capabilities.
/// Use within a <c>await using</c> block to ensure the lock is released.
/// </summary>
public interface IDistributedLock
{
    /// <summary>
    /// Attempts to acquire a named distributed lock.
    /// </summary>
    /// <param name="name">The unique name identifying the lock resource.</param>
    /// <param name="timeout">
    /// Maximum time to wait for the lock.
    /// Use <see cref="TimeSpan.Zero"/> (default) for an immediate, non-blocking attempt.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the acquisition attempt.</param>
    /// <returns>
    /// A handle to the acquired lock, or <see langword="null"/> if the lock
    /// could not be acquired within the specified <paramref name="timeout"/>.
    /// </returns>
    Task<IDistributedLockHandle?> TryAcquireAsync(
        string name,
        TimeSpan timeout = default,
        CancellationToken cancellationToken = default);
}
