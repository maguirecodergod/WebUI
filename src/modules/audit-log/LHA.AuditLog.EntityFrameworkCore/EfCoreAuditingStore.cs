using LHA.Auditing;
using LHA.AuditLog.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IAuditingStore"/> that persists audit log entries
/// to the <see cref="AuditLogDbContext"/> database.
/// <para>
/// Replaces the default <see cref="LoggingAuditingStore"/> when the Audit Log module
/// is registered. Uses the <see cref="AuditLogFactory"/> to convert in-memory
/// <see cref="AuditLogEntry"/> POCOs into persistent domain entities.
/// </para>
/// </summary>
public sealed class EfCoreAuditingStore : IAuditingStore
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EfCoreAuditingStore> _logger;

    public EfCoreAuditingStore(
        IServiceScopeFactory scopeFactory,
        ILogger<EfCoreAuditingStore> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SaveAsync(AuditLogEntry auditLogEntry, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use a dedicated scope to avoid polluting the current request's DbContext
            // and to ensure the audit log is saved even if the request fails.
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuditLogDbContext>();

            var entity = AuditLogFactory.Create(auditLogEntry);

            dbContext.AuditLogs.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug(
                "Audit log saved: {AuditLogId}, User={UserId}, Url={Url}, Duration={Duration}ms",
                entity.Id, entity.UserId, entity.Url, entity.ExecutionDuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save audit log entry for URL: {Url}", auditLogEntry.Url);
        }
    }
}
