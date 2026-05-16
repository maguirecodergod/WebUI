namespace LHA.Notification.Domain.Shared;

public sealed class RedisSettings
{
    public const string SectionName = "Redis";
    public string? ConnectionString { get; set; }
    public string InstanceName { get; set; } = "notification:";
    public int Database { get; set; } = 1;
}
