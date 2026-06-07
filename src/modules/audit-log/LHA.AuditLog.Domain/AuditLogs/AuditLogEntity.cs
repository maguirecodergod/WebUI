using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Persistent audit log aggregate — the core entity of the Audit Log bounded context.
/// <para>
/// This is a write-once, read-many entity. Unlike typical aggregates, it does not need
/// modification tracking, soft-delete, or concurrency stamps. Created exclusively by
/// <c>IAuditingStore</c> during request processing.
/// </para>
/// </summary>
[DisableAuditing]
public sealed class AuditLogEntity : Entity<Guid>, IMultiTenant, IHasCreationTime
{
    private readonly List<AuditLogActionEntity> _actions = [];
    private readonly List<EntityChangeEntity> _entityChanges = [];

    // ─── Properties ──────────────────────────────────────────────────

    /// <summary>Application/service name that produced this log.</summary>
    public string? ApplicationName { get; private init; }

    /// <summary>Name of the action (e.g. "Login", "Register").</summary>
    public string? ActionName { get; private init; }

    /// <summary>Request/transport type that produced this log.</summary>
    public CRequestType RequestType { get; private init; }

    /// <summary>Authenticated user identifier.</summary>
    public Guid? UserId { get; private init; }

    /// <summary>Authenticated user display name.</summary>
    public string? UserName { get; private init; }

    /// <summary>Tenant identifier.</summary>
    public Guid? TenantId { get; private init; }

    /// <summary>Tenant display name.</summary>
    public string? TenantName { get; private init; }

    /// <summary>Impersonator user identifier (admin-as-user scenarios).</summary>
    public Guid? ImpersonatorUserId { get; private init; }

    /// <summary>Impersonator tenant identifier.</summary>
    public Guid? ImpersonatorTenantId { get; private init; }

    /// <summary>UTC time of the request/operation start.</summary>
    public DateTimeOffset ExecutionTime { get; private init; }

    /// <summary>Total duration in milliseconds.</summary>
    public int ExecutionDuration { get; private init; }

    /// <summary>Client/API-key identifier.</summary>
    public string? ClientId { get; private init; }

    /// <summary>Distributed correlation identifier.</summary>
    public string? CorrelationId { get; private init; }

    /// <summary>Client IP address.</summary>
    public string? ClientIpAddress { get; private init; }

    /// <summary>HTTP method (GET, POST, etc.).</summary>
    public string? HttpMethod { get; private init; }

    /// <summary>HTTP response status code.</summary>
    public int? HttpStatusCode { get; private init; }

    /// <summary>Request URL.</summary>
    public string? Url { get; private init; }

    /// <summary>Browser/user agent information.</summary>
    public string? BrowserInfo { get; private init; }

    /// <summary>Serialized exception messages (JSON array).</summary>
    public string? Exceptions { get; private init; }

    /// <summary>Serialized comments (JSON array).</summary>
    public string? Comments { get; private init; }

    /// <summary>Serialized extra properties (JSON object).</summary>
    public string? ExtraProperties { get; private init; }

    /// <summary>Creation time (same as ExecutionTime for audit logs).</summary>
    public DateTimeOffset CreationTime { get; private init; }

    /// <summary>Method invocations tracked in this request.</summary>
    public IReadOnlyCollection<AuditLogActionEntity> Actions => _actions.AsReadOnly();

    /// <summary>Entity changes tracked during this request.</summary>
    public IReadOnlyCollection<EntityChangeEntity> EntityChanges => _entityChanges.AsReadOnly();

    // ─── Constructors ────────────────────────────────────────────────

    /// <summary>EF Core constructor.</summary>
    private AuditLogEntity() { }

    /// <summary>
    /// Creates a new audit log entity from the framework's <see cref="AuditLogEntry"/>.
    /// </summary>
    public AuditLogEntity(
        Guid id,
        string? applicationName,
        Guid? userId,
        string? userName,
        Guid? tenantId,
        string? tenantName,
        Guid? impersonatorUserId,
        Guid? impersonatorTenantId,
        DateTimeOffset executionTime,
        int executionDuration,
        CRequestType requestType,
        string? clientId,
        string? correlationId,
        string? clientIpAddress,
        string? httpMethod,
        int? httpStatusCode,
        string? url,
        string? browserInfo,
        string? actionName,
        string? exceptions,
        string? comments,
        string? extraProperties)
    {
        Id = id;
        ApplicationName = applicationName;
        UserId = userId;
        UserName = userName;
        TenantId = tenantId;
        TenantName = tenantName;
        ImpersonatorUserId = impersonatorUserId;
        ImpersonatorTenantId = impersonatorTenantId;
        ExecutionTime = executionTime;
        ExecutionDuration = executionDuration;
        RequestType = requestType;
        ClientId = clientId;
        CorrelationId = correlationId;
        ClientIpAddress = clientIpAddress;
        HttpMethod = httpMethod;
        HttpStatusCode = httpStatusCode;
        Url = url;
        BrowserInfo = browserInfo;
        ActionName = actionName;
        Exceptions = exceptions;
        Comments = comments;
        ExtraProperties = extraProperties;
        CreationTime = executionTime;
    }

    // ─── Sub-entity management (internal) ────────────────────────────

    /// <summary>Adds an action to this audit log.</summary>
    public void AddAction(AuditLogActionEntity action)
    {
        _actions.Add(action);
    }

    /// <summary>Adds an entity change to this audit log.</summary>
    public void AddEntityChange(EntityChangeEntity entityChange)
    {
        _entityChanges.Add(entityChange);
    }
}
