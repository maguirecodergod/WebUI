namespace LHA.Notification.Domain.Shared;

public sealed class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";
    public int DefaultPerUserPerMinute { get; set; } = 60;
    public int DefaultPerTenantPerMinute { get; set; } = 10000;
    public int DefaultPerTenantPerDay { get; set; } = 1000000;
}
