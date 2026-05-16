using LHA.Shared.Domain.Enums.Notification;
using LHA.Shared.Domain.Notification;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Notification;

public record ChannelConfigurationDto(
    Guid Id,
    Guid? TenantId,
    CNotificationChannel Channel,
    CProviderType ProviderType,
    bool IsEnabled,
    ProviderSettings? Settings = null)
{
    public SmtpProviderSettings? ToSmtpSettings() => Settings as SmtpProviderSettings;
    public AwsSesProviderSettings? ToAwsSesSettings() => Settings as AwsSesProviderSettings;
    public SendGridProviderSettings? ToSendGridSettings() => Settings as SendGridProviderSettings;
    public TwilioProviderSettings? ToTwilioSettings() => Settings as TwilioProviderSettings;
    public AwsSnsProviderSettings? ToAwsSnsSettings() => Settings as AwsSnsProviderSettings;
    public FcmProviderSettings? ToFcmSettings() => Settings as FcmProviderSettings;
    public ApnsProviderSettings? ToApnsSettings() => Settings as ApnsProviderSettings;
    public WebPushProviderSettings? ToWebPushSettings() => Settings as WebPushProviderSettings;
}

public class CreateUpdateChannelConfigurationDto
{
    public Guid? TenantId { get; set; }
    public CNotificationChannel Channel { get; set; }
    public CProviderType ProviderType { get; set; }
    public ProviderSettings? Settings { get; set; }
    public bool IsEnabled { get; set; } = true;

    public CreateUpdateChannelConfigurationDto() { }

    public CreateUpdateChannelConfigurationDto(CNotificationChannel channel, CProviderType providerType, ProviderSettings? settings, bool isEnabled = true)
    {
        Channel = channel;
        ProviderType = providerType;
        Settings = settings;
        IsEnabled = isEnabled;
    }
}

public class GetChannelConfigurationsInput : PagedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? TenantId { get; set; }
    public CNotificationChannel? Channel { get; set; }
    public CProviderType? ProviderType { get; set; }
    public bool? IsEnabled { get; set; }
}
