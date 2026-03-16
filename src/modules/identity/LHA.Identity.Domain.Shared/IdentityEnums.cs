namespace LHA.Identity.Domain.Shared;

/// <summary>
/// Represents the result of a login attempt.
/// </summary>
public enum LoginResultType
{
    /// <summary>Login succeeded.</summary>
    Success = 1,

    /// <summary>Invalid username or email.</summary>
    InvalidUserNameOrEmail = 2,

    /// <summary>Invalid password.</summary>
    InvalidPassword = 3,

    /// <summary>User account is not active / disabled.</summary>
    NotAllowed = 4,

    /// <summary>User account is locked out due to excessive failed attempts.</summary>
    LockedOut = 5,

    /// <summary>
    /// Two-factor authentication is required but not yet completed.
    /// </summary>
    RequiresTwoFactor = 6,

    /// <summary>User's tenant is inactive.</summary>
    TenantIsNotActive = 7
}

/// <summary>
/// Value types for <c>IdentityClaimType</c>.
/// </summary>
public enum IdentityClaimValueType
{
    /// <summary>Free-form string.</summary>
    String = 0,

    /// <summary>Integer value.</summary>
    Int = 1,

    /// <summary>Boolean (true / false).</summary>
    Boolean = 2,

    /// <summary>ISO-8601 date/time.</summary>
    DateTime = 3
}

/// <summary>
/// The provider that grants a permission.
/// <c>"R"</c> = role, <c>"U"</c> = user.
/// </summary>
public static class PermissionGrantProviderName
{
    /// <summary>Permission granted to a role.</summary>
    public const string Role = "R";

    /// <summary>Permission granted to a specific user.</summary>
    public const string User = "U";
}

/// <summary>
/// Security log identity constants (the "source" of the action).
/// </summary>
public static class IdentitySecurityLogIdentityConsts
{
    public const string Identity = "Identity";
    public const string IdentityExternal = "IdentityExternal";
    public const string IdentityTwoFactor = "IdentityTwoFactor";
}

/// <summary>
/// Security log action constants.
/// </summary>
public static class IdentitySecurityLogActionConsts
{
    public const string LoginSucceeded = "LoginSucceeded";
    public const string LoginFailed = "LoginFailed";
    public const string LoginNotAllowed = "LoginNotAllowed";
    public const string LoginLockedout = "LoginLockedout";
    public const string LoginRequiresTwoFactor = "LoginRequiresTwoFactor";
    public const string Logout = "Logout";
    public const string ChangePassword = "ChangePassword";
    public const string TokenRefreshed = "TokenRefreshed";
}
