namespace LHA.Identity.Application.Contracts;

/// <summary>
/// Permission name constants for the Identity module.
/// These match the permission definitions seeded in the database.
/// </summary>
public static class IdentityPermissions
{
    public static class Users
    {
        public const string Read = "identity.users.read";
        public const string Create = "identity.users.create";
        public const string Update = "identity.users.update";
        public const string Delete = "identity.users.delete";
    }

    public static class Roles
    {
        public const string Read = "identity.roles.read";
        public const string Create = "identity.roles.create";
        public const string Update = "identity.roles.update";
        public const string Delete = "identity.roles.delete";
    }

    public static class ClaimTypes
    {
        public const string Read = "identity.claim-types.read";
        public const string Create = "identity.claim-types.create";
        public const string Update = "identity.claim-types.update";
        public const string Delete = "identity.claim-types.delete";
    }

    public static class SecurityLogs
    {
        public const string Read = "identity.security-logs.read";
    }
}
