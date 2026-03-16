using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.Auditing;

/// <summary>
/// Default <see cref="IAuditingStore"/> that writes audit logs to the configured logger.
/// Used as the fallback before a persistent store is registered.
/// </summary>
internal sealed class LoggingAuditingStore : IAuditingStore
{
    private readonly ILogger<LoggingAuditingStore> _logger;

    public LoggingAuditingStore(ILogger<LoggingAuditingStore>? logger = null)
    {
        _logger = logger ?? NullLogger<LoggingAuditingStore>.Instance;
    }

    public Task SaveAsync(AuditLogEntry auditLog, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("{AuditLog}", auditLog.ToString());
        return Task.CompletedTask;
    }
}
