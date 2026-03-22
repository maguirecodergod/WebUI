namespace LHA.Core.Users;

/// <summary>
/// Low-level accessor for the ambient current-user information,
/// analogous to <see cref="LHA.MultiTenancy.ICurrentTenantAccessor"/>.
/// <para>
/// Backed by <see cref="AsyncLocal{T}"/> so that user context
/// flows correctly across async operations.
/// Typically used by background jobs or system operations that need
/// to impersonate a specific user.
/// </para>
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    /// Gets or sets the current user info.
    /// <c>null</c> means no user context has been established (anonymous).
    /// </summary>
    BasicUserInfo? Current { get; set; }
}

/// <summary>
/// Lightweight snapshot of user identity for ambient propagation.
/// </summary>
/// <param name="UserId">User identifier.</param>
/// <param name="UserName">User login name.</param>
/// <param name="TenantId">Tenant identifier (<c>null</c> for host users).</param>
/// <param name="Roles">Comma-separated role names.</param>
public sealed record BasicUserInfo(
    Guid UserId,
    string? UserName = null,
    Guid? TenantId = null,
    string[]? Roles = null);
