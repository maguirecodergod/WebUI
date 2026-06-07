namespace LHA.AuditLog.Domain.Shared
{
    /// <summary>
    /// Controls which audit subsystems are wired to EF Core storage.
    /// </summary>
    [Flags]
    public enum CAuditLogStoreMode
    {
        /// <summary>Structured relational audit log (AuditLog / Action / EntityChange tables).</summary>
        DataAudit = 1,

        /// <summary>High-throughput pipeline audit log (AuditLogPipeline table).</summary>
        Pipeline = 2,

        /// <summary>Both subsystems active simultaneously.</summary>
        All = DataAudit | Pipeline
    }
}