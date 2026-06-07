using LHA.Shared.Domain.Enums.Notification;
using LHA.Shared.Domain.Notification;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.Notification;

/// <summary>
/// Represents a notification channel configuration with provider settings.
/// </summary>
/// <param name="Id">Unique identifier of the channel configuration.</param>
/// <param name="TenantId">Tenant scope of the configuration. <c>null</c> for host-level configurations.</param>
/// <param name="Channel">Notification channel type (e.g., Email, Sms, Push).</param>
/// <param name="ProviderType">Provider implementation type (e.g., <c>Smtp</c>, <c>AwsSes</c>, <c>SendGrid</c>).</param>
/// <param name="IsEnabled">Indicates whether this channel configuration is currently active.</param>
/// <param name="Settings">Provider-specific settings serialized for the configured provider type.</param>
public record ChannelConfigurationDto(
    Guid Id,
    Guid? TenantId,
    CNotificationChannel Channel,
    CProviderType ProviderType,
    bool IsEnabled,
    ProviderSettings? Settings = null)
{
    /// <summary>
    /// Attempts to cast the current settings to <see cref="SmtpProviderSettings"/>.
    /// </summary>
    public SmtpProviderSettings? ToSmtpSettings() => Settings as SmtpProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="AwsSesProviderSettings"/>.
    /// </summary>
    public AwsSesProviderSettings? ToAwsSesSettings() => Settings as AwsSesProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="SendGridProviderSettings"/>.
    /// </summary>
    public SendGridProviderSettings? ToSendGridSettings() => Settings as SendGridProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="TwilioProviderSettings"/>.
    /// </summary>
    public TwilioProviderSettings? ToTwilioSettings() => Settings as TwilioProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="AwsSnsProviderSettings"/>.
    /// </summary>
    public AwsSnsProviderSettings? ToAwsSnsSettings() => Settings as AwsSnsProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="FcmProviderSettings"/>.
    /// </summary>
    public FcmProviderSettings? ToFcmSettings() => Settings as FcmProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="ApnsProviderSettings"/>.
    /// </summary>
    public ApnsProviderSettings? ToApnsSettings() => Settings as ApnsProviderSettings;

    /// <summary>
    /// Attempts to cast the current settings to <see cref="WebPushProviderSettings"/>.
    /// </summary>
    public WebPushProviderSettings? ToWebPushSettings() => Settings as WebPushProviderSettings;
}

/// <summary>
/// Input for creating or updating a notification channel configuration.
/// </summary>
public class CreateUpdateChannelConfigurationDto
{
    /// <summary>
    /// Tenant scope of the configuration. <c>null</c> for host-level configurations.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Notification channel type (e.g., Email, Sms, Push).
    /// </summary>
    public CNotificationChannel Channel { get; set; }

    /// <summary>
    /// Provider implementation type (e.g., <c>Smtp</c>, <c>AwsSes</c>, <c>SendGrid</c>).
    /// </summary>
    public CProviderType ProviderType { get; set; }

    /// <summary>
    /// Provider-specific settings for the configured provider type.
    /// </summary>
    public ProviderSettings? Settings { get; set; }

    /// <summary>
    /// Indicates whether this channel configuration is enabled. Defaults to <c>true</c>.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Initializes a new instance with default values.
    /// </summary>
    public CreateUpdateChannelConfigurationDto() { }

    /// <summary>
    /// Initializes a new instance with the specified channel, provider type, settings, and enabled state.
    /// </summary>
    public CreateUpdateChannelConfigurationDto(CNotificationChannel channel, CProviderType providerType, ProviderSettings? settings, bool isEnabled = true)
    {
        Channel = channel;
        ProviderType = providerType;
        Settings = settings;
        IsEnabled = isEnabled;
    }
}

/// <summary>
/// Input for querying channel configurations with optional filters and pagination.
/// </summary>
public class GetChannelConfigurationsInput : PagedResultRequestDto
{
    /// <summary>
    /// Optional text filter to search channel configurations.
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Optional tenant filter. <c>null</c> returns host-level configurations.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Optional filter by notification channel type.
    /// </summary>
    public CNotificationChannel? Channel { get; set; }

    /// <summary>
    /// Optional filter by provider type.
    /// </summary>
    public CProviderType? ProviderType { get; set; }

    /// <summary>
    /// Optional filter by enabled status.
    /// </summary>
    public bool? IsEnabled { get; set; }
}
