namespace LHA.Shared.Domain.Enums.Notification;

public enum CNotificationChannel
{
    /// <summary>
    /// 0 - InApp
    /// </summary>
    InApp = 0,
    /// <summary>
    /// 1 - Push
    /// </summary>
    Push = 1,
    /// <summary>
    /// 2 - Email
    /// </summary>
    Email = 2,
    /// <summary>
    /// 3 - Sms
    /// </summary>
    Sms = 3,
    /// <summary>
    /// 4 - WebPush
    /// </summary>
    WebPush = 4,
    /// <summary>
    /// 5 - Webhook
    /// </summary>
    Webhook = 5
}

public enum CProviderType
{
    /// <summary>
    /// 0 - Fcm
    /// </summary>
    Fcm,
    /// <summary>
    /// 1 - Apns
    /// </summary>
    Apns,
    /// <summary>
    /// 2 - SendGrid
    /// </summary>
    SendGrid,
    /// <summary>
    /// 3 - AwsSes
    /// </summary>
    AwsSes,
    /// <summary>
    /// 4 - Smtp
    /// </summary>
    Smtp,
    /// <summary>
    /// 5 - Twilio
    /// </summary>
    Twilio,
    /// <summary>
    /// 6 - AwsSns
    /// </summary>
    AwsSns,
    /// <summary>
    /// 7 - WebPush
    /// </summary>
    WebPush,
    /// <summary>
    /// 8 - Internal
    /// </summary>
    Internal
}
