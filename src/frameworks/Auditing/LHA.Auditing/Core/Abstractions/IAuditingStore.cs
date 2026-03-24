namespace LHA.Auditing;

/// <summary>
/// Persists <see cref="AuditLogEntry"/> records.
/// Implement for your data store (EF Core, Elasticsearch, cloud logging, etc.).
/// </summary>
public interface IAuditingStore
{
    /// <summary>Persists a completed audit log entry.</summary>
    Task SaveAsync(AuditLogEntry auditLog, CancellationToken cancellationToken = default);
}
