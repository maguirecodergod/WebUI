namespace LHA.Core.Users;

/// <summary>
/// Provides read-only access to the current (authenticated) user context.
/// <para>
/// Implementations vary by hosting model:
/// <list type="bullet">
///   <item>ASP.NET Core → reads <c>HttpContext.User</c> (ClaimsPrincipal)</item>
///   <item>Background workers → reads from <c>AsyncLocal</c> accessor</item>
///   <item>gRPC / message consumers → reads from metadata / message headers</item>
/// </list>
/// </para>
/// <para>
/// Register as <b>Scoped</b> (one per request or scope) so that the
/// identity stays consistent within a single operation.
/// </para>
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Whether the current context represents an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Unique identifier of the current user.
    /// <c>null</c> when the user is anonymous.
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// Login name of the current user.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Display name (first name) of the current user.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Surname (last name) of the current user.
    /// </summary>
    string? Surname { get; }

    /// <summary>
    /// Email address of the current user.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Whether the user's email has been confirmed.
    /// </summary>
    bool EmailVerified { get; }

    /// <summary>
    /// Phone number of the current user.
    /// </summary>
    string? PhoneNumber { get; }

    /// <summary>
    /// Whether the user's phone number has been confirmed.
    /// </summary>
    bool PhoneNumberVerified { get; }

    /// <summary>
    /// Tenant identifier of the current user.
    /// <c>null</c> for host-level (super-admin) users.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Roles assigned to the current user (claim-based).
    /// </summary>
    string[] Roles { get; }

    /// <summary>
    /// Finds the first claim value matching the given <paramref name="claimType"/>.
    /// </summary>
    string? FindClaimValue(string claimType);

    /// <summary>
    /// Finds all claim values matching the given <paramref name="claimType"/>.
    /// </summary>
    string[] FindClaimValues(string claimType);

    /// <summary>
    /// Whether the current user has the given <paramref name="roleName"/>.
    /// </summary>
    bool IsInRole(string roleName);
}
