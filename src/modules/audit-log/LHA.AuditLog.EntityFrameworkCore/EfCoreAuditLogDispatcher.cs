using LHA.AuditLog.Domain;
using LHA.Auditing.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// <see cref="IAuditLogDispatcher"/> that persists Pipeline audit log records
/// directly to the database via EF Core using the unified <see cref="AuditLogDbContext"/>.
/// </summary>
internal sealed class EfCoreAuditLogDispatcher : IAuditLogDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EfCoreAuditLogDispatcher> _logger;

    public EfCoreAuditLogDispatcher(
        IServiceScopeFactory scopeFactory,
        ILogger<EfCoreAuditLogDispatcher> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IReadOnlyList<AuditLogRecord> records, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditLogDbContext>();

        var entities = new List<AuditLogPipelineEntity>(records.Count);

        foreach (var record in records)
        {
            entities.Add(new AuditLogPipelineEntity
            {
                Id = record.Id,
                Timestamp = record.Timestamp,
                DurationMs = record.DurationMs,
                ServiceName = record.ServiceName,
                InstanceId = record.InstanceId,
                ActionName = record.ActionName,
                ActionType = (byte)record.ActionType,
                RequestType = (byte)record.RequestType,
                UserId = record.UserId,
                TenantId = record.TenantId,
                UserName = record.UserName,
                Roles = record.Roles,
                TraceId = record.TraceId,
                SpanId = record.SpanId,
                CorrelationId = record.CorrelationId,
                Status = (byte)record.Status,
                StatusCode = record.StatusCode,
                HttpMethod = record.HttpMethod,
                RequestPath = record.RequestPath,
                RequestBody = record.RequestBody,
                ResponseBody = record.ResponseBody,
                ClientIp = record.ClientIp,
                UserAgent = record.UserAgent,
                Exception = record.Exception,
                Tags = record.Tags
            });
        }

        dbContext.AuditLogPipeline.AddRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Dispatched {Count} pipeline audit logs to database.", records.Count);
    }
}
