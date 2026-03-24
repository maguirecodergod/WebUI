namespace LHA.Auditing;

/// <summary>
/// Null-object <see cref="IAuditUserProvider"/> that returns anonymous context.
/// Used as the default before a real identity provider is registered.
/// </summary>
internal sealed class NullAuditUserProvider : IAuditUserProvider
{
    public Guid? UserId => null;
    public string? UserName => null;
    public Guid? TenantId => null;
    public bool IsAuthenticated => false;
}
