namespace LHA.Notification.Domain.Shared;

public static class CacheKeys
{
    public const string UnreadCountPrefix = "unread:";
    public const string DeviceTokenPrefix = "device:token:";
    public const string NotificationTemplatePrefix = "template:";
    public const string UserPreferencePrefix = "user:preference:";
    public const string TenantChannelConfigPrefix = "tenant:channel:";
    public const string RateLimitPrefix = "rateLimit:";
    public const string BatchProgressPrefix = "batch:progress:";
    public const string UnreadCountKeyFormat = "{0}:{1}";
    public const string DeviceTokenKeyFormat = "{0}:{1}";
    public const string TemplateKeyFormat = "{0}:{1}";
    public const string PreferenceKeyFormat = "{0}:{1}";
    public const string TenantConfigKeyFormat = "{0}:{1}";
    public const string RateLimitKeyFormat = "{0}:{1}";
    public const string BatchProgressKeyFormat = "{0}:{1}";
}
