namespace LHA.Shared.Contracts;

/// <summary>
/// Centralizes event topic names used for cross-service messaging and integration events.
/// </summary>
public static class EventTopics
{
    /// <summary>
    /// Topic for account-related events such as user registration, login, and profile changes.
    /// </summary>
    public const string AccountEvents = "account-events";

    /// <summary>
    /// Topic for notification-related events such as delivery status updates and channel events.
    /// </summary>
    public const string NotificationEvents = "notification-events";

    /// <summary>
    /// Topic for mega-promotion and campaign-related events.
    /// </summary>
    public const string MegaEvents = "mega-events";
}
