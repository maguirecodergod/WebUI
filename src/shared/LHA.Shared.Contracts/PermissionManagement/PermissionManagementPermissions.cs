namespace LHA.Shared.Contracts.PermissionManagement;

/// <summary>
/// Permission name constants for the Permission Management module.
/// </summary>
public static class PermissionManagementPermissions
{
    public static class Definitions
    {
        public const string Read = "permissions.definitions.read";
        public const string Manage = "permissions.definitions.manage";
    }

    public static class Groups
    {
        public const string Read = "permissions.groups.read";
        public const string Manage = "permissions.groups.manage";
    }

    public static class Templates
    {
        public const string Read = "permissions.templates.read";
        public const string Manage = "permissions.templates.manage";
    }

    public static class Grants
    {
        public const string Read = "permissions.grants.read";
        public const string Manage = "permissions.grants.manage";
    }
}
