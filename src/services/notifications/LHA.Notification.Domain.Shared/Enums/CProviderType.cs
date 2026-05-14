namespace LHA.Notification.Domain.Shared;

public enum CProviderType
{
    Fcm,
    Apns,
    SendGrid,
    AwsSes,
    Smtp,
    Twilio,
    AwsSns,
    WebPush,
    Internal
}