using LHA.Auditing;
using LHA.Core.Users;

namespace LHA.AspNetCore.Security;

/// <summary>
/// Bridges <see cref="ICurrentUser"/> to <see cref="IAuditUserProvider"/>
/// so that the auditing framework automatically picks up the authenticated
/// user from the current request context.
/// <para>
/// Replaces the <see cref="NullAuditUserProvider"/> that ships as the
/// default fallback in <c>LHA.Auditing</c>.
/// </para>
/// </summary>
internal sealed class HttpContextAuditUserProvider : IAuditUserProvider
{
    private readonly ICurrentUser _currentUser;

    public HttpContextAuditUserProvider(ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        _currentUser = currentUser;
    }

    /// <inheritdoc />
    public Guid? UserId => _currentUser.Id;

    /// <inheritdoc />
    public string? UserName => _currentUser.UserName;

    /// <inheritdoc />
    public Guid? TenantId => _currentUser.TenantId;

    /// <inheritdoc />
    public bool IsAuthenticated => _currentUser.IsAuthenticated;
}
