namespace LHA.Shared.Domain.Enums.Notification;

public enum CNotificationChannel
{
    InApp = 0,
    Push = 1,
    Email = 2,
    Sms = 3,
    WebPush = 4,
    Webhook = 5
}

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
