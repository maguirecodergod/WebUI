namespace LHA.Notification.Infrastructure.Persistences;

internal static class DbSchemeConsts
{
    public const string ServiceName = "Notification";

    private const string Sep = "_";

    // ─── Group prefixes ──────────────────────────────────────────
    private const string PNotify = "Notify" + Sep;
    private const string PAudit  = "Audit"  + Sep;
    private const string PEvent  = "Event"  + Sep;

    // ─── Notification Domain ─────────────────────────────────────
    public static class Notification
    {
        public const string Notifications          = PNotify + "Notification";
        public const string Templates              = PNotify + "Template";
        public const string UserPreferences        = PNotify + "UserPreference";
        public const string TenantChannelConfigs   = PNotify + "TenantChannelConfig";
        public const string Devices                = PNotify + "Device";
        public const string NotificationBatches    = PNotify + "NotificationBatch";
    }

    // ─── Audit Log ────────────────────────────────────────────
    public static class Audit
    {
        public const string Log            = PAudit + "Log";
        public const string Action         = PAudit + "Action";
        public const string EntityChange   = PAudit + "EntityChange";
        public const string PropertyChange = PAudit + "PropertyChange";
        public const string LogPipeline    = PAudit + "LogPipeline";
    }

    // ─── Event Bus ───────────────────────────────────────────────
    public static class Event
    {
        public const string Outbox = PEvent + "Outbox";
        public const string Inbox  = PEvent + "Inbox";
    }
}
