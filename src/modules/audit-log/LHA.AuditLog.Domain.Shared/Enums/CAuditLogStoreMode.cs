namespace LHA.AuditLog.Domain.Shared
{
    /// <summary>
    /// Controls which audit subsystems are wired to EF Core storage.
    /// </summary>
    [Flags]
    public enum CAuditLogStoreMode
    {
        /// <summary>
        /// 1 - DataAudit: Structured relational audit log (AuditLog / Action / EntityChange tables).
        /// </summary>
        DataAudit = 1,

        /// <summary>
        /// 2 - Pipeline: High-throughput pipeline audit log (AuditLogPipeline table).
        /// </summary>
        Pipeline = 2,

        /// <summary>Both subsystems active simultaneously.</summary>
        All = DataAudit | Pipeline
    }
}