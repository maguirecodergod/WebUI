namespace LHA.DistributedLocking;

/// <summary>
/// Lock handle for the in-process <see cref="LocalDistributedLock"/>.
/// </summary>
internal sealed class LocalDistributedLockHandle : IDistributedLockHandle
{
    private readonly IDisposable _releaser;

    public LocalDistributedLockHandle(IDisposable releaser)
    {
        _releaser = releaser;
    }

    /// <inheritdoc />
    /// <remarks>Always <see cref="CancellationToken.None"/> — local locks cannot be lost.</remarks>
    public CancellationToken HandleLostToken => CancellationToken.None;

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _releaser.Dispose();
        return default;
    }
}
