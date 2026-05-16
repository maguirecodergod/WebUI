namespace LHA.Notification.Domain.Shared;

public static class NotificationConstants
{
    public const int MaxRetries = 3;
    public const int MaxBulkBatchSize = 500;
    public const int NotificationCacheTtlMinutes = 30;
    public const int UnreadCountCacheTtlHours = 24;
    public const int DeviceTokenCacheTtlMinutes = 60;
    public const int RateLimitDefaultPerUserPerMinute = 60;
    public const int RateLimitDefaultPerTenantPerMinute = 10000;
    public const int RateLimitDefaultPerTenantPerDay = 1000000;
    public const int FcmBatchSize = 500;
    public const int TimeoutSeconds = 10;
}
