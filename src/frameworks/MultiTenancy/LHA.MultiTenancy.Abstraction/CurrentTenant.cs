namespace LHA.MultiTenancy;

/// <summary>
/// Default <see cref="ICurrentTenant"/> implementation using
/// <see cref="ICurrentTenantAccessor"/> for ambient storage.
/// </summary>
internal sealed class CurrentTenant : ICurrentTenant
{
    private readonly ICurrentTenantAccessor _accessor;

    public CurrentTenant(ICurrentTenantAccessor accessor)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        _accessor = accessor;
    }

    /// <inheritdoc />
    public bool IsAvailable => Id.HasValue;

    /// <inheritdoc />
    public Guid? Id => _accessor.Current?.TenantId;

    /// <inheritdoc />
    public string? Name => _accessor.Current?.Name;

    /// <inheritdoc />
    public IDisposable Change(Guid? id, string? name = null)
    {
        var previous = _accessor.Current;
        _accessor.Current = new BasicTenantInfo(id, name);
        return new TenantScope(_accessor, previous);
    }

    private sealed class TenantScope(ICurrentTenantAccessor accessor, BasicTenantInfo? previous) : IDisposable
    {
        public void Dispose() => accessor.Current = previous;
    }
}
