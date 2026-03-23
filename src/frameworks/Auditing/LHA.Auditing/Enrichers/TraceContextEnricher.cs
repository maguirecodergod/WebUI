using System.Diagnostics;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Enriches audit log records with OpenTelemetry trace context (TraceId, SpanId).
/// </summary>
internal sealed class TraceContextEnricher : IAuditLogEnricher
{
    public void Enrich(AuditLogRecord record)
    {
        var activity = Activity.Current;
        if (activity is null)
            return;

        record.TraceId ??= activity.TraceId.ToString();
        record.SpanId ??= activity.SpanId.ToString();
    }
}
