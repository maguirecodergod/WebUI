namespace LHA.Notification.Domain.Shared;

public sealed class SignalRSettings
{
    public const string SectionName = "SignalR";
    public required string RedisBackplaneConnection { get; set; }
}
