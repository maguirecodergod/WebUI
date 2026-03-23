namespace LHA.Auditing.Pipeline;

/// <summary>
/// Dispatches a batch of audit log records to a storage backend.
/// <para>
/// Implementations must be idempotent — the batch processor may
/// retry a batch on transient failures.
/// </para>
/// </summary>
public interface IAuditLogDispatcher
{
    /// <summary>
    /// Dispatches a batch of audit log records.
    /// </summary>
    Task DispatchAsync(IReadOnlyList<AuditLogRecord> records, CancellationToken cancellationToken = default);
}
