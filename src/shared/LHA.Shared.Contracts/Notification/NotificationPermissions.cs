namespace LHA.Shared.Contracts.Notification;

public static class NotificationPermissions
{
    public const string GroupName = "Notification";

    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string Send = Default + ".Send";
        public const string Read = Default + ".Read";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Notification.Notifications.Default";
            public const string Send = "Permissions.Notification.Notifications.Send";
            public const string Read = "Permissions.Notification.Notifications.Read";
            public const string Delete = "Permissions.Notification.Notifications.Delete";
        }
    }

    public static class Devices
    {
        public const string Default = GroupName + ".Devices";
        public const string Read = Default + ".Read";
        public const string Manage = Default + ".Manage";

        public static class L
        {
            public const string Default = "Permissions.Notification.Devices.Default";
            public const string Read = "Permissions.Notification.Devices.Read";
            public const string Manage = "Permissions.Notification.Devices.Manage";
        }
    }

    public static class Templates
    {
        public const string Default = GroupName + ".Templates";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Notification.Templates.Default";
            public const string Read = "Permissions.Notification.Templates.Read";
            public const string Create = "Permissions.Notification.Templates.Create";
            public const string Update = "Permissions.Notification.Templates.Update";
            public const string Delete = "Permissions.Notification.Templates.Delete";
        }
    }

    public static class Batches
    {
        public const string Default = GroupName + ".Batches";
        public const string Read = Default + ".Read";
        public const string Create = Default + ".Create";
        public const string Manage = Default + ".Manage";

        public static class L
        {
            public const string Default = "Permissions.Notification.Batches.Default";
            public const string Read = "Permissions.Notification.Batches.Read";
            public const string Create = "Permissions.Notification.Batches.Create";
            public const string Manage = "Permissions.Notification.Batches.Manage";
        }
    }

    public static class Preferences
    {
        public const string Default = GroupName + ".Preferences";
        public const string Read = Default + ".Read";
        public const string Update = Default + ".Update";

        public static class L
        {
            public const string Default = "Permissions.Notification.Preferences.Default";
            public const string Read = "Permissions.Notification.Preferences.Read";
            public const string Update = "Permissions.Notification.Preferences.Update";
        }
    }

    public static class Configuration
    {
        public const string Default = GroupName + ".Configuration";
        public const string Read = Default + ".Read";
        public const string Manage = Default + ".Manage";

        public static class L
        {
            public const string Default = "Permissions.Notification.Configuration.Default";
            public const string Read = "Permissions.Notification.Configuration.Read";
            public const string Manage = "Permissions.Notification.Configuration.Manage";
        }
    }

    public static class Statistics
    {
        public const string Default = GroupName + ".Statistics";
        public const string Read = Default + ".Read";

        public static class L
        {
            public const string Default = "Permissions.Notification.Statistics.Default";
            public const string Read = "Permissions.Notification.Statistics.Read";
        }
    }

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string Read = Default + ".Read";
        public const string Delete = Default + ".Delete";

        public static class L
        {
            public const string Default = "Permissions.Notification.AuditLogs.Default";
            public const string Read = "Permissions.Notification.AuditLogs.Read";
            public const string Delete = "Permissions.Notification.AuditLogs.Delete";
        }
    }

    public static class L
    {
        public const string Group = "Permissions.Notification.Group";
    }
}
