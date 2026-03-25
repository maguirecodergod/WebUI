namespace LHA.Auditing
{
    /// <summary>
    /// Mode defining which auditing mechanisms (Producers) to enable across the pipeline.
    /// </summary>
    [Flags]
    public enum CAuditingMode
    {
        /// <summary>Relational Data Action and Entity Logs</summary>
        DataAudit = 1,

        /// <summary>High-throughput API logs pipeline</summary>
        Pipeline = 2,

        /// <summary>Both auditing mechanisms</summary>
        All = DataAudit | Pipeline
    }
}