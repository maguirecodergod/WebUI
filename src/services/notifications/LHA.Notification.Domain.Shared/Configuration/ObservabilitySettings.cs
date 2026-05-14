namespace LHA.Notification.Domain.Shared;

public sealed class ObservabilitySettings
{
    public const string SectionName = "Observability";
    public string ServiceName { get; set; } = "LHA.Notification";
    public string? OtlpEndpoint { get; set; }
}
