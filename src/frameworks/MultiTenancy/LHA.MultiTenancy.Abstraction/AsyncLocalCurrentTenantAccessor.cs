namespace LHA.MultiTenancy;

/// <summary>
/// <see cref="ICurrentTenantAccessor"/> backed by <see cref="AsyncLocal{T}"/>
/// for safe ambient tenant propagation across async flows.
/// </summary>
internal sealed class AsyncLocalCurrentTenantAccessor : ICurrentTenantAccessor
{
    private readonly AsyncLocal<BasicTenantInfo?> _currentScope = new();

    public BasicTenantInfo? Current
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }
}
