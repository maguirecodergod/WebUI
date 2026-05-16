using System.Text.Json.Serialization;

namespace LHA.Shared.Domain.Notification;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SmtpProviderSettings), "Smtp")]
[JsonDerivedType(typeof(AwsSesProviderSettings), "AwsSes")]
[JsonDerivedType(typeof(SendGridProviderSettings), "SendGrid")]
[JsonDerivedType(typeof(TwilioProviderSettings), "Twilio")]
[JsonDerivedType(typeof(AwsSnsProviderSettings), "AwsSns")]
[JsonDerivedType(typeof(FcmProviderSettings), "Fcm")]
[JsonDerivedType(typeof(ApnsProviderSettings), "Apns")]
[JsonDerivedType(typeof(WebPushProviderSettings), "WebPush")]
public abstract record ProviderSettings;

public record SmtpProviderSettings(
    string Host,
    int Port,
    string? Username = null,
    string? Password = null,
    bool UseSsl = true,
    string? FromEmail = null,
    string? FromName = null) : ProviderSettings
{
    public const string SectionName = "Notification:Email:Smtp";
    public int PoolSize { get; init; } = 10;
}

public record AwsSesProviderSettings(
    string AccessKey,
    string SecretKey,
    string Region,
    string? FromEmail = null,
    string? Endpoint = null) : ProviderSettings
{
    public const string SectionName = "Notification:Email:AwsSes";
}

public record SendGridProviderSettings(
    string ApiKey,
    string? FromEmail = null,
    string? FromName = null,
    string? ReplyToEmail = null,
    string? ReplyToName = null,
    bool SandboxMode = false,
    bool ClickTracking = true,
    bool OpenTracking = true,
    bool SubscriptionTracking = false,
    bool BypassListManagement = false,
    string? Endpoint = null,
    string? WebhookPublicKey = null) : ProviderSettings
{
    public const string SectionName = "Notification:Email:SendGrid";
}

public record TwilioProviderSettings(
    string AccountSid,
    string AuthToken,
    string FromPhoneNumber,
    string? Endpoint = null) : ProviderSettings
{
    public const string SectionName = "Notification:Sms:Twilio";
}

public record AwsSnsProviderSettings(
    string AccessKey,
    string SecretKey,
    string Region) : ProviderSettings
{
    public const string SectionName = "Notification:Sms:AwsSns";
}

public record FcmProviderSettings(
    string ProjectId,
    string? ServiceAccountJson = null,
    string? Endpoint = null) : ProviderSettings
{
    public const string SectionName = "Notification:Push:Fcm";
}

public record ApnsProviderSettings(
    string BundleId,
    string P8Certificate,
    string KeyId,
    string TeamId,
    bool IsProduction = false,
    string? Endpoint = null) : ProviderSettings
{
    public const string SectionName = "Notification:Push:Apns";
}

public record WebPushProviderSettings(
    string PublicKey,
    string PrivateKey,
    string Subject) : ProviderSettings
{
    public const string SectionName = "Notification:WebPush";
}
