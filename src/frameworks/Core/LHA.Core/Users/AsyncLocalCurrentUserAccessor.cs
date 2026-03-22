namespace LHA.Core.Users;

/// <summary>
/// <see cref="ICurrentUserAccessor"/> backed by <see cref="AsyncLocal{T}"/>
/// for safe ambient user propagation across async flows.
/// <para>
/// Registered as <b>Singleton</b>. The <see cref="AsyncLocal{T}"/> ensures
/// each async context gets its own copy.
/// </para>
/// </summary>
internal sealed class AsyncLocalCurrentUserAccessor : ICurrentUserAccessor
{
    private readonly AsyncLocal<BasicUserInfo?> _currentScope = new();

    public BasicUserInfo? Current
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }
}
