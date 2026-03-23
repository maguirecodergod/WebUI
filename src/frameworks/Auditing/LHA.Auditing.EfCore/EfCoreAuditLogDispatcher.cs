using System.Text.Json;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Auditing.EfCore;

/// <summary>
/// <see cref="IAuditLogDispatcher"/> that persists audit log records
/// directly to the database via EF Core.
/// <para>
/// Used as a reliable fallback when Kafka is unavailable, or as the
/// primary storage in simpler deployments.
/// Uses a dedicated DbContext scope to avoid polluting the request pipeline.
/// Performs bulk insert for efficiency.
/// </para>
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
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditLogPipelineDbContext>();

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

        dbContext.AuditLogs.AddRange(entities);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Dispatched {Count} audit logs to database.", records.Count);
    }
}

/// <summary>
/// Lightweight EF Core entity for the pipeline audit log records.
/// Separate from the existing AuditLogEntity to avoid coupling.
/// </summary>
public sealed class AuditLogPipelineEntity
{
    public string Id { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; }
    public long DurationMs { get; set; }
    public string? ServiceName { get; set; }
    public string? InstanceId { get; set; }
    public string? ActionName { get; set; }
    public byte ActionType { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? UserName { get; set; }
    public string? Roles { get; set; }
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
    public string? CorrelationId { get; set; }
    public byte Status { get; set; }
    public int? StatusCode { get; set; }
    public string? HttpMethod { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public string? Exception { get; set; }
    public string? Tags { get; set; }
}

/// <summary>
/// DbContext for pipeline audit log storage.
/// </summary>
public sealed class AuditLogPipelineDbContext : DbContext
{
    public DbSet<AuditLogPipelineEntity> AuditLogs => Set<AuditLogPipelineEntity>();

    public AuditLogPipelineDbContext(DbContextOptions<AuditLogPipelineDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLogPipelineEntity>(b =>
        {
            b.ToTable("AuditLogPipeline");
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).HasMaxLength(26); // ULID length
            b.Property(e => e.ServiceName).HasMaxLength(256);
            b.Property(e => e.InstanceId).HasMaxLength(256);
            b.Property(e => e.ActionName).HasMaxLength(512);
            b.Property(e => e.UserId).HasMaxLength(40);
            b.Property(e => e.TenantId).HasMaxLength(40);
            b.Property(e => e.UserName).HasMaxLength(256);
            b.Property(e => e.TraceId).HasMaxLength(64);
            b.Property(e => e.SpanId).HasMaxLength(64);
            b.Property(e => e.CorrelationId).HasMaxLength(128);
            b.Property(e => e.HttpMethod).HasMaxLength(10);
            b.Property(e => e.RequestPath).HasMaxLength(2048);
            b.Property(e => e.ClientIp).HasMaxLength(64);
            b.Property(e => e.UserAgent).HasMaxLength(512);

            // Index for common queries
            b.HasIndex(e => e.Timestamp);
            b.HasIndex(e => e.TenantId);
            b.HasIndex(e => e.UserId);
            b.HasIndex(e => e.TraceId);
            b.HasIndex(e => new { e.ServiceName, e.Timestamp });
        });
    }
}
