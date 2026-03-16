namespace LHA.PermissionManagement.Domain.Shared;

/// <summary>
/// Constants for the <c>PermissionDefinition</c> entity (Layer 1 — atomic permissions).
/// </summary>
public static class PermissionDefinitionConsts
{
    public const int MaxNameLength = 256;
    public const int MaxDisplayNameLength = 256;
    public const int MaxServiceNameLength = 128;
    public const int MaxGroupNameLength = 128;
    public const int MaxDescriptionLength = 512;
}

/// <summary>
/// Constants for the <c>PermissionGroup</c> entity (Layer 2).
/// </summary>
public static class PermissionGroupConsts
{
    public const int MaxNameLength = 128;
    public const int MaxDisplayNameLength = 256;
    public const int MaxServiceNameLength = 128;
    public const int MaxDescriptionLength = 512;
}

/// <summary>
/// Constants for the <c>PermissionTemplate</c> entity (Layer 3).
/// </summary>
public static class PermissionTemplateConsts
{
    public const int MaxNameLength = 128;
    public const int MaxDisplayNameLength = 256;
    public const int MaxDescriptionLength = 512;
}

/// <summary>
/// Constants for the <c>PermissionGrant</c> entity.
/// </summary>
public static class PermissionGrantConsts
{
    public const int MaxProviderNameLength = 64;
    public const int MaxProviderKeyLength = 256;
}
