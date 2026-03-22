namespace LHA.Core.Users;

/// <summary>
/// Convenience extension methods for <see cref="ICurrentUser"/>.
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Returns the full display name by combining <see cref="ICurrentUser.Name"/>
    /// and <see cref="ICurrentUser.Surname"/>.
    /// </summary>
    public static string? GetFullName(this ICurrentUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Name) && string.IsNullOrWhiteSpace(user.Surname))
            return user.UserName;

        return string.Join(' ',
            new[] { user.Name, user.Surname }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    /// <summary>
    /// Returns the user ID or throws if the user is not authenticated.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there is no authenticated user in the current context.
    /// </exception>
    public static Guid GetId(this ICurrentUser user)
    {
        return user.Id ?? throw new InvalidOperationException(
            "There is no authenticated user in the current context. " +
            "Call this method only in authenticated endpoints.");
    }

    /// <summary>
    /// Returns <c>true</c> if the current user is a host-level user
    /// (not associated with any tenant).
    /// </summary>
    public static bool IsHost(this ICurrentUser user) =>
        user.IsAuthenticated && !user.TenantId.HasValue;

    /// <summary>
    /// Returns <c>true</c> if the current user belongs to a specific tenant.
    /// </summary>
    public static bool BelongsToTenant(this ICurrentUser user, Guid tenantId) =>
        user.TenantId == tenantId;

    /// <summary>
    /// Returns <c>true</c> if the current user is the built-in system user.
    /// </summary>
    public static bool IsSystem(this ICurrentUser user) =>
        CurrentUserDefaults.IsSystemIdentity(user.Id);

    /// <summary>
    /// Returns <c>true</c> if the current user is the built-in admin user.
    /// </summary>
    public static bool IsAdmin(this ICurrentUser user) =>
        CurrentUserDefaults.IsAdminUser(user.Id) || user.IsInRole(CurrentUserDefaults.AdminRoleName);
}
