namespace LHA.Shared.Contracts.AuditLog;

/// <summary>
/// Permission name constants for the Audit Log module.
/// </summary>
public static class AuditLogPermissions
{
    public static class AuditLogs
    {
        public const string Read = "audit-logs.read";

        /// <summary>Host-level permission to view ALL audit logs cross-tenant.</summary>
        public const string HostRead = "audit-logs.host-read";
    }
}
