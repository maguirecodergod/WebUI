namespace LHA.Shared.Contracts.Realtime;

public static class SignalRHubMethodNames
{
    public static class NotificationEvents
    {
        public const string Received = "notification.received";
        public const string Read = "notification.read";
        public const string Deleted = "notification.deleted";
        public const string UnreadCountUpdated = "unread.count.updated";
    }

    public static class BatchEvents
    {
        public const string ProgressUpdated = "batch.progress.updated";
    }

    public static class SecurityEvents
    {
        public const string ForceLogout = "system.forceLogout";
    }
}
