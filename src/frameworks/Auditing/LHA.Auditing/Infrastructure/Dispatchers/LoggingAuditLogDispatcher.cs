using System.Text.Json;
using LHA.Auditing.Serialization;
using Microsoft.Extensions.Logging;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Debug/fallback <see cref="IAuditLogDispatcher"/> that writes audit log records
/// to structured logging (Serilog / ILogger).
/// <para>
/// Used as the default dispatcher and as a fallback when the circuit breaker opens.
/// </para>
/// </summary>
internal sealed class LoggingAuditLogDispatcher : IAuditLogDispatcher
{
    private readonly ILogger<LoggingAuditLogDispatcher> _logger;

    public LoggingAuditLogDispatcher(ILogger<LoggingAuditLogDispatcher> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task DispatchAsync(IReadOnlyList<AuditLogRecord> records, CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            _logger.LogInformation(
                "[AUDIT] {ActionType} {ActionName} | User={UserId} | Tenant={TenantId} | " +
                "Status={Status} | StatusCode={StatusCode} | Duration={DurationMs}ms | " +
                "IP={ClientIp} | TraceId={TraceId} | CorrelationId={CorrelationId}",
                record.ActionType,
                record.ActionName,
                record.UserId,
                record.TenantId,
                record.Status,
                record.StatusCode,
                record.DurationMs,
                record.ClientIp,
                record.TraceId,
                record.CorrelationId);

            if (record.Exception is not null)
            {
                _logger.LogWarning("[AUDIT EXCEPTION] {ActionName}: {Exception}",
                    record.ActionName, record.Exception);
            }
        }

        return Task.CompletedTask;
    }
}
