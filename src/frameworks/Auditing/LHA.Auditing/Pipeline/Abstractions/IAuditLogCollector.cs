namespace LHA.Auditing.Pipeline;

/// <summary>
/// Collects audit log records from interceptors and feeds them into the pipeline.
/// Applies enrichment and masking before buffering.
/// </summary>
public interface IAuditLogCollector
{
    /// <summary>
    /// Submits a completed audit log record to the pipeline.
    /// This method must be non-blocking and thread-safe.
    /// </summary>
    /// <returns><c>true</c> if the record was accepted; <c>false</c> if dropped.</returns>
    bool Collect(AuditLogRecord record);
}
