namespace LHA.Core.Users;

/// <summary>
/// Null-object <see cref="ICurrentUser"/> that represents an anonymous (unauthenticated) context.
/// Used as the default fallback before a real identity provider is registered.
/// </summary>
internal sealed class NullCurrentUser : ICurrentUser
{
    public bool IsAuthenticated => false;
    public Guid? Id => null;
    public string? UserName => null;
    public string? Name => null;
    public string? Surname => null;
    public string? Email => null;
    public bool EmailVerified => false;
    public string? PhoneNumber => null;
    public bool PhoneNumberVerified => false;
    public Guid? TenantId => null;
    public string[] Roles => [];

    public string? FindClaimValue(string claimType) => null;
    public string[] FindClaimValues(string claimType) => [];
    public bool IsInRole(string roleName) => false;
}
