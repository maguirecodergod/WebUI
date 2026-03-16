namespace LHA.Account.EntityFrameworkCore;

internal static class DbSchemeConsts
{
    public const string ServiceName = "Account";

    private const string Sep = "_";

    // ─── Group prefixes ──────────────────────────────────────────
    private const string PAuth       = "Auth"       + Sep;
    private const string PTenant     = "Tenant"     + Sep;
    private const string PAudit      = "Audit"      + Sep;
    private const string PPermission = "Permission" + Sep;
    private const string PEvent      = "Event"      + Sep;

    // ─── Identity (Auth) ─────────────────────────────────────────
    public static class Auth
    {
        public const string User            = PAuth + "User";
        public const string UserRole        = PAuth + "UserRole";
        public const string UserClaim       = PAuth + "UserClaim";
        public const string UserLogin       = PAuth + "UserLogin";
        public const string UserToken       = PAuth + "UserToken";
        public const string Role            = PAuth + "Role";
        public const string RoleClaim       = PAuth + "RoleClaim";
        public const string ClaimType       = PAuth + "ClaimType";
        public const string SecurityLog     = PAuth + "SecurityLog";
        public const string PermissionGrant = PAuth + "PermissionGrant";
    }

    // ─── Tenant Management ───────────────────────────────────────
    public static class Tenant
    {
        public const string Main             = PTenant + "Tenant";
        public const string ConnectionString = PTenant + "ConnectionString";
    }

    // ─── Audit Log ───────────────────────────────────────────────
    public static class Audit
    {
        public const string Log            = PAudit + "Log";
        public const string Action         = PAudit + "Action";
        public const string EntityChange   = PAudit + "EntityChange";
        public const string PropertyChange = PAudit + "PropertyChange";
    }

    // ─── Permission Management ───────────────────────────────────
    public static class Permission
    {
        public const string Definition   = PPermission + "Definition";
        public const string PermGroup    = PPermission + "Group";
        public const string GroupItem    = PPermission + "GroupItem";
        public const string Template     = PPermission + "Template";
        public const string TemplateItem = PPermission + "TemplateItem";
        public const string Grant        = PPermission + "Grant";
    }

    // ─── Event Bus ───────────────────────────────────────────────
    public static class Event
    {
        public const string Outbox = PEvent + "Outbox";
        public const string Inbox  = PEvent + "Inbox";
    }
}