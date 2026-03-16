namespace LHA.Identity.Domain.Shared;

/// <summary>
/// Constants for <c>IdentityUser</c> entity field constraints.
/// </summary>
public static class IdentityUserConsts
{
    public const int MaxUserNameLength = 256;
    public const int MaxEmailLength = 256;
    public const int MaxPhoneNumberLength = 16;
    public const int MaxPasswordHashLength = 256;
    public const int MaxSecurityStampLength = 64;
    public const int MaxNameLength = 64;
    public const int MaxSurnameLength = 64;
}

/// <summary>
/// Constants for <c>IdentityRole</c> entity field constraints.
/// </summary>
public static class IdentityRoleConsts
{
    public const int MaxNameLength = 256;
}

/// <summary>
/// Constants for <c>IdentityClaimType</c> entity field constraints.
/// </summary>
public static class IdentityClaimTypeConsts
{
    public const int MaxNameLength = 256;
    public const int MaxRegexLength = 512;
    public const int MaxRegexDescriptionLength = 128;
    public const int MaxDescriptionLength = 256;
}

/// <summary>
/// Constants for claim values and claim type strings.
/// </summary>
public static class IdentityClaimConsts
{
    public const int MaxClaimTypeLength = 256;
    public const int MaxClaimValueLength = 1024;
}

/// <summary>
/// Constants for <c>IdentityUserLogin</c> entity field constraints.
/// </summary>
public static class IdentityUserLoginConsts
{
    public const int MaxLoginProviderLength = 64;
    public const int MaxProviderKeyLength = 196;
    public const int MaxProviderDisplayNameLength = 128;
}

/// <summary>
/// Constants for <c>IdentityUserToken</c> entity field constraints.
/// </summary>
public static class IdentityUserTokenConsts
{
    public const int MaxLoginProviderLength = 64;
    public const int MaxNameLength = 128;
    public const int MaxValueLength = 2048;
}

/// <summary>
/// Constants for <c>IdentitySecurityLog</c> entity field constraints.
/// </summary>
public static class IdentitySecurityLogConsts
{
    public const int MaxApplicationNameLength = 96;
    public const int MaxIdentityLength = 96;
    public const int MaxActionLength = 96;
    public const int MaxUserNameLength = 256;
    public const int MaxTenantNameLength = 64;
    public const int MaxClientIdLength = 64;
    public const int MaxCorrelationIdLength = 64;
    public const int MaxClientIpAddressLength = 64;
    public const int MaxBrowserInfoLength = 512;
    public const int MaxExtraPropertiesLength = 4096;
}

/// <summary>
/// Constants for <c>IdentityPermissionGrant</c> entity field constraints.
/// </summary>
public static class IdentityPermissionGrantConsts
{
    public const int MaxNameLength = 128;
    public const int MaxProviderNameLength = 64;
    public const int MaxProviderKeyLength = 64;
}
