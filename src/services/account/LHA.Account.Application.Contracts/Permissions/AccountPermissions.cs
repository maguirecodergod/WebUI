using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.TenantManagement;
using LHA.Shared.Contracts.AuditLog;
using LHA.Shared.Contracts.PermissionManagement;

namespace LHA.Account.Application.Contracts.Permissions;

/// <summary>
/// Unified permission constants for the Account service.
/// These constants are used for authorization in endpoints and for localization display.
/// </summary>
public static class AccountPermissions
{
    public static class UserManagement
    {
        public const string GroupName = "UserManagement";
        public const string Read = IdentityPermissions.Users.Read;
        public const string Create = IdentityPermissions.Users.Create;
        public const string Update = IdentityPermissions.Users.Update;
        public const string Delete = IdentityPermissions.Users.Delete;

        public static class L
        {
            public const string Group = "Permissions.UserManagement.Group";
            public const string Read = "Permissions.UserManagement.Read";
            public const string Create = "Permissions.UserManagement.Create";
            public const string Update = "Permissions.UserManagement.Update";
            public const string Delete = "Permissions.UserManagement.Delete";
        }
    }

    public static class RoleManagement
    {
        public const string GroupName = "RoleManagement";
        public const string Read = IdentityPermissions.Roles.Read;
        public const string Create = IdentityPermissions.Roles.Create;
        public const string Update = IdentityPermissions.Roles.Update;
        public const string Delete = IdentityPermissions.Roles.Delete;

        public static class L
        {
            public const string Group = "Permissions.RoleManagement.Group";
            public const string Read = "Permissions.RoleManagement.Read";
            public const string Create = "Permissions.RoleManagement.Create";
            public const string Update = "Permissions.RoleManagement.Update";
            public const string Delete = "Permissions.RoleManagement.Delete";
        }
    }

    public static class ClaimTypeManagement
    {
        public const string GroupName = "ClaimTypeManagement";
        public const string Read = IdentityPermissions.ClaimTypes.Read;
        public const string Create = IdentityPermissions.ClaimTypes.Create;
        public const string Update = IdentityPermissions.ClaimTypes.Update;
        public const string Delete = IdentityPermissions.ClaimTypes.Delete;

        public static class L
        {
            public const string Group = "Permissions.ClaimTypeManagement.Group";
            public const string Read = "Permissions.ClaimTypeManagement.Read";
            public const string Create = "Permissions.ClaimTypeManagement.Create";
            public const string Update = "Permissions.ClaimTypeManagement.Update";
            public const string Delete = "Permissions.ClaimTypeManagement.Delete";
        }
    }

    public static class SecurityLogManagement
    {
        public const string GroupName = "SecurityLogManagement";
        public const string Read = IdentityPermissions.SecurityLogs.Read;

        public static class L
        {
            public const string Group = "Permissions.SecurityLogManagement.Group";
            public const string Read = "Permissions.SecurityLogManagement.Read";
        }
    }

    public static class TenantManagement
    {
        public const string GroupName = "TenantManagement";
        public const string Read = TenantManagementPermissions.Tenants.Read;
        public const string Create = TenantManagementPermissions.Tenants.Create;
        public const string Update = TenantManagementPermissions.Tenants.Update;
        public const string Delete = TenantManagementPermissions.Tenants.Delete;

        public static class L
        {
            public const string Group = "Permissions.TenantManagement.Group";
            public const string Read = "Permissions.TenantManagement.Read";
            public const string Create = "Permissions.TenantManagement.Create";
            public const string Update = "Permissions.TenantManagement.Update";
            public const string Delete = "Permissions.TenantManagement.Delete";
        }
    }

    public static class AuditLogManagement
    {
        public const string GroupName = "AuditLogManagement";
        public const string Read = AuditLogPermissions.AuditLogs.Read;

        public static class L
        {
            public const string Group = "Permissions.AuditLogManagement.Group";
            public const string Read = "Permissions.AuditLogManagement.Read";
        }
    }

    public static class PermissionMgmt
    {
        public const string GroupName = "PermissionMgmt";
        public const string DefinitionsRead = PermissionManagementPermissions.Definitions.Read;
        public const string DefinitionsManage = PermissionManagementPermissions.Definitions.Manage;
        public const string GroupsRead = PermissionManagementPermissions.Groups.Read;
        public const string GroupsManage = PermissionManagementPermissions.Groups.Manage;
        public const string TemplatesRead = PermissionManagementPermissions.Templates.Read;
        public const string TemplatesManage = PermissionManagementPermissions.Templates.Manage;
        public const string GrantsRead = PermissionManagementPermissions.Grants.Read;
        public const string GrantsManage = PermissionManagementPermissions.Grants.Manage;

        public static class L
        {
            public const string Group = "Permissions.PermissionMgmt.Group";
            public const string DefinitionsRead = "Permissions.PermissionMgmt.DefinitionsRead";
            public const string DefinitionsManage = "Permissions.PermissionMgmt.DefinitionsManage";
            public const string GroupsRead = "Permissions.PermissionMgmt.GroupsRead";
            public const string GroupsManage = "Permissions.PermissionMgmt.GroupsManage";
            public const string TemplatesRead = "Permissions.PermissionMgmt.TemplatesRead";
            public const string TemplatesManage = "Permissions.PermissionMgmt.TemplatesManage";
            public const string GrantsRead = "Permissions.PermissionMgmt.GrantsRead";
            public const string GrantsManage = "Permissions.PermissionMgmt.GrantsManage";
        }
    }

    public static class Templates
    {
        public static class TenantAdmin
        {
            public const string Name = "TenantAdmin";
            public const string L_Name = "Permissions.Templates.TenantAdmin.Name";
            public const string L_Description = "Permissions.Templates.TenantAdmin.Description";
        }
    }
}
