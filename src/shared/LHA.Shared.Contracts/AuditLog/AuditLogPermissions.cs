namespace LHA.Shared.Contracts.AuditLog;

/// <summary>
/// Permission name constants for the Audit Log module.
/// </summary>
public static class AuditLogPermissions
{
    public static class AuditLogs
    {
        public const string Read   = "audit-logs.read";
        public const string Create = "audit-logs.create";
        public const string Update = "audit-logs.update";
        public const string Delete = "audit-logs.delete";
    }
}
