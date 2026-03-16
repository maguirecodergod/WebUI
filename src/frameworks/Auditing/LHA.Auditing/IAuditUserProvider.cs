namespace LHA.Auditing;

/// <summary>
/// Provides the current user identity for audit property setting.
/// <para>
/// Implement this interface in the application layer to bridge with your
/// identity/authentication system (e.g., <c>ClaimsPrincipal</c>).
/// </para>
/// </summary>
public interface IAuditUserProvider
{
    /// <summary>Identifier of the currently authenticated user (<c>null</c> if anonymous).</summary>
    Guid? UserId { get; }

    /// <summary>Display name of the current user.</summary>
    string? UserName { get; }

    /// <summary>Tenant identifier of the current user (<c>null</c> for host users).</summary>
    Guid? TenantId { get; }

    /// <summary>Whether the current user is authenticated.</summary>
    bool IsAuthenticated { get; }
}
