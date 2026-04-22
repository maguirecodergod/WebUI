namespace LHA.Mega.Application.Contracts.Permissions;

/// <summary>
/// Unified permission constants for the Mega service.
/// These constants are used for authorization in endpoints and for localization display.
/// </summary>
public static class MegaPermissions
{
    public static class MegaAccountManagement
    {
        public const string GroupName = "MegaAccountManagement";
        public const string Read = "mega.accounts.read";
        public const string Create = "mega.accounts.create";
        public const string Update = "mega.accounts.update";
        public const string Delete = "mega.accounts.delete";

        public static class L
        {
            public const string Group = "Permissions.MegaAccountManagement.Group";
            public const string Read = "Permissions.MegaAccountManagement.Read";
            public const string Create = "Permissions.MegaAccountManagement.Create";
            public const string Update = "Permissions.MegaAccountManagement.Update";
            public const string Delete = "Permissions.MegaAccountManagement.Delete";
        }
    }
}