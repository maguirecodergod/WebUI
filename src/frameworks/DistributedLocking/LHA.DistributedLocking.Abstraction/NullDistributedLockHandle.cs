namespace LHA.DistributedLocking;

/// <summary>
/// A no-op lock handle returned by <see cref="NullDistributedLock"/>.
/// </summary>
internal sealed class NullDistributedLockHandle : IDistributedLockHandle
{
    internal static readonly NullDistributedLockHandle Instance = new();

    private NullDistributedLockHandle() { }

    /// <inheritdoc />
    public CancellationToken HandleLostToken => CancellationToken.None;

    /// <inheritdoc />
    public ValueTask DisposeAsync() => default;
}
