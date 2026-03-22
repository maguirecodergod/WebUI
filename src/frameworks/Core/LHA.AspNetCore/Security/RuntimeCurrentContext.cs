using LHA.Core.Users;
using LHA.MultiTenancy;

namespace LHA.AspNetCore.Security;

/// <summary>
/// Unified runtime context that combines <see cref="ICurrentUser"/> and
/// <see cref="ICurrentTenant"/> into a single injectable service.
/// <para>
/// Use this in Application Services and Domain Services that need both
/// user and tenant context for their operations.
/// </para>
/// <para>
/// Also provides convenience methods for temporary scope switching
/// — e.g., switching to a different user or tenant for a background operation.
/// </para>
/// </summary>
public sealed class RuntimeCurrentContext
{
    /// <summary>
    /// The current authenticated user.
    /// </summary>
    public ICurrentUser User { get; }

    /// <summary>
    /// The current tenant.
    /// </summary>
    public ICurrentTenant Tenant { get; }

    private readonly ICurrentUserAccessor _userAccessor;

    public RuntimeCurrentContext(
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        ICurrentUserAccessor currentUserAccessor)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(currentTenant);
        ArgumentNullException.ThrowIfNull(currentUserAccessor);

        User = currentUser;
        Tenant = currentTenant;
        _userAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Whether the context has both an authenticated user and a resolved tenant.
    /// </summary>
    public bool IsFullyResolved => User.IsAuthenticated && Tenant.IsAvailable;

    /// <summary>
    /// Temporarily impersonates a different user for the duration of the
    /// returned <see cref="IDisposable"/> scope.
    /// <para>
    /// Useful for background jobs or system operations that need to
    /// appear as a specific user in audit logs.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// using (context.ChangeUser(systemUserId, "system"))
    /// {
    ///     // All operations here will be attributed to the system user
    ///     await repository.InsertAsync(entity);
    /// }
    /// </code>
    /// </example>
    public IDisposable ChangeUser(Guid userId, string? userName = null, Guid? tenantId = null)
    {
        var previous = _userAccessor.Current;
        _userAccessor.Current = new BasicUserInfo(userId, userName, tenantId);
        return new UserScope(_userAccessor, previous);
    }

    /// <summary>
    /// Temporarily impersonates the system user.
    /// </summary>
    public IDisposable ChangeToSystemUser()
    {
        return ChangeUser(
            CurrentUserDefaults.SystemUserId,
            CurrentUserDefaults.SystemUserName);
    }

    /// <summary>
    /// Temporarily switches to a different tenant.
    /// Delegates to <see cref="ICurrentTenant.Change"/>.
    /// </summary>
    public IDisposable ChangeTenant(Guid? tenantId, string? name = null)
    {
        return Tenant.Change(tenantId, name);
    }

    /// <summary>
    /// Temporarily switches to both a different user and tenant.
    /// Dispose the returned handle to restore both scopes.
    /// </summary>
    public IDisposable Change(Guid userId, string? userName, Guid? tenantId, string? tenantName = null)
    {
        var userScope = ChangeUser(userId, userName, tenantId);
        var tenantScope = Tenant.Change(tenantId, tenantName);
        return new CombinedScope(userScope, tenantScope);
    }

    // ─── Private scope helpers ──────────────────────────────────

    private sealed class UserScope(ICurrentUserAccessor accessor, BasicUserInfo? previous) : IDisposable
    {
        public void Dispose() => accessor.Current = previous;
    }

    private sealed class CombinedScope(IDisposable userScope, IDisposable tenantScope) : IDisposable
    {
        public void Dispose()
        {
            tenantScope.Dispose();
            userScope.Dispose();
        }
    }
}
