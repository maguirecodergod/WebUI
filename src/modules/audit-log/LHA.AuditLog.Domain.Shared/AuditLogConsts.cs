namespace LHA.AuditLog.Domain.Shared;

/// <summary>
/// Constants for the Audit Log module — max lengths, defaults, etc.
/// </summary>
public static class AuditLogConsts
{
    public const int MaxApplicationNameLength = 96;
    public const int MaxUserNameLength = 256;
    public const int MaxTenantNameLength = 64;
    public const int MaxClientIdLength = 64;
    public const int MaxCorrelationIdLength = 64;
    public const int MaxClientIpAddressLength = 64;
    public const int MaxHttpMethodLength = 16;
    public const int MaxUrlLength = 2048;
    public const int MaxBrowserInfoLength = 512;
    public const int MaxServiceNameLength = 512;
    public const int MaxMethodNameLength = 256;
    public const int MaxEntityIdLength = 128;
    public const int MaxEntityTypeFullNameLength = 512;
    public const int MaxPropertyNameLength = 128;
    public const int MaxPropertyTypeFullNameLength = 512;
    public const int MaxPropertyValueLength = 2048;

    /// <summary>Default retention period for audit logs (days).</summary>
    public const int DefaultRetentionDays = 90;
}
