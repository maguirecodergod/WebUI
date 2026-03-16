namespace LHA.DistributedLocking;

/// <summary>
/// Configuration options for the distributed locking infrastructure.
/// </summary>
public sealed class DistributedLockOptions
{
    /// <summary>
    /// A prefix prepended to every lock key.
    /// Useful for isolating lock namespaces in shared stores.
    /// Default is <see cref="string.Empty"/>.
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Default timeout for lock acquisition when none is explicitly specified.
    /// Default is <see cref="TimeSpan.Zero"/> (immediate attempt, no waiting).
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.Zero;
}
