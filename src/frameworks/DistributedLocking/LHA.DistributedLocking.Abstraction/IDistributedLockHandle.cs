namespace LHA.DistributedLocking;

/// <summary>
/// Represents an acquired distributed lock.
/// Dispose the handle to release the lock.
/// </summary>
public interface IDistributedLockHandle : IAsyncDisposable
{
    /// <summary>
    /// A token that is triggered if the underlying lock is lost while being held
    /// (e.g., due to a connection failure with the lock store).
    /// For in-process locks this is always <see cref="CancellationToken.None"/>.
    /// </summary>
    CancellationToken HandleLostToken { get; }
}
