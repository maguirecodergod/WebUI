namespace LHA.Auditing.Pipeline;

/// <summary>
/// Enriches an <see cref="AuditLogRecord"/> with additional metadata.
/// <para>
/// Implementations are executed in registration order.
/// Keep enrichers lightweight — they run synchronously in the request path.
/// </para>
/// </summary>
public interface IAuditLogEnricher
{
    /// <summary>
    /// Enriches the audit log record with additional data.
    /// </summary>
    void Enrich(AuditLogRecord record);
}
