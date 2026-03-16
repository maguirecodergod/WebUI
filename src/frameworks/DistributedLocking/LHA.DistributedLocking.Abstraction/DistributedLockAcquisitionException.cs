namespace LHA.DistributedLocking;

/// <summary>
/// Thrown when a distributed lock cannot be acquired within the allowed timeout.
/// </summary>
public sealed class DistributedLockAcquisitionException : Exception
{
    /// <summary>
    /// The name of the lock that could not be acquired.
    /// </summary>
    public string LockName { get; }

    /// <summary>
    /// The timeout that was exceeded.
    /// </summary>
    public TimeSpan Timeout { get; }

    public DistributedLockAcquisitionException(string lockName, TimeSpan timeout)
        : base($"Failed to acquire distributed lock '{lockName}' within {timeout}.")
    {
        LockName = lockName;
        Timeout = timeout;
    }
}
