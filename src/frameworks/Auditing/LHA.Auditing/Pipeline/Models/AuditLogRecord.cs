using LHA;
using LHA.Auditing;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Flattened, analytics-friendly audit log record.
/// Designed for high-throughput serialization and minimal GC pressure.
/// <para>
/// This record is the unit of data flowing through the audit pipeline:
/// Interceptor → Collector → Buffer → Batcher → Dispatcher.
/// </para>
/// </summary>
public sealed class AuditLogRecord
{
    /// <summary>Unique identifier (ULID for time-ordered sorting).</summary>
    public string Id { get; set; } = Ulid.NewUlid().ToString();

    /// <summary>UTC timestamp when the operation started.</summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>Operation duration in milliseconds.</summary>
    public long DurationMs { get; set; }

    /// <summary>Name of the service/application producing this log.</summary>
    public string? ServiceName { get; set; }

    /// <summary>Instance identifier for horizontal scaling (pod/container id).</summary>
    public string? InstanceId { get; set; }

    /// <summary>Name of the action (endpoint route, gRPC method, job type, event handler).</summary>
    public string? ActionName { get; set; }

    /// <summary>Type of action that was audited.</summary>
    public AuditActionType ActionType { get; set; }

    /// <summary>Request/transport type that produced this log.</summary>
    public CRequestType RequestType { get; set; } = CRequestType.Unknown;

    /// <summary>Authenticated user identifier.</summary>
    public string? UserId { get; set; }

    /// <summary>Tenant identifier.</summary>
    public string? TenantId { get; set; }

    /// <summary>Authenticated user display name.</summary>
    public string? UserName { get; set; }

    /// <summary>Comma-separated user roles.</summary>
    public string? Roles { get; set; }

    /// <summary>OpenTelemetry trace identifier.</summary>
    public string? TraceId { get; set; }

    /// <summary>OpenTelemetry span identifier.</summary>
    public string? SpanId { get; set; }

    /// <summary>Distributed correlation identifier.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Operation outcome.</summary>
    public AuditLogStatus Status { get; set; }

    /// <summary>HTTP status code (if HTTP request).</summary>
    public int? StatusCode { get; set; }

    /// <summary>HTTP method (GET, POST, etc.).</summary>
    public string? HttpMethod { get; set; }

    /// <summary>Request path / URL.</summary>
    public string? RequestPath { get; set; }

    /// <summary>Serialized request body (JSON, masked, optionally compressed).</summary>
    public string? RequestBody { get; set; }

    /// <summary>Serialized response body (optional, configurable).</summary>
    public string? ResponseBody { get; set; }

    /// <summary>Client IP address.</summary>
    public string? ClientIp { get; set; }

    /// <summary>User agent / browser info.</summary>
    public string? UserAgent { get; set; }

    /// <summary>Structured exception information (JSON).</summary>
    public string? Exception { get; set; }

    /// <summary>Key-value tags for extensible metadata (JSON).</summary>
    public string? Tags { get; set; }
}
