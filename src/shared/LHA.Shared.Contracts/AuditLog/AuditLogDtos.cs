using LHA.Auditing;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.AuditLog;

// ─── AuditLog DTOs ───────────────────────────────────────────────

/// <summary>
/// DTO for a single audit log entry.
/// </summary>
public sealed class AuditLogDto : EntityDto<Guid>
{
    /// <summary>
    /// Name of the application or microservice that produced the audit entry (e.g., <c>Account</c>, <c>Notification</c>).
    /// </summary>
    public string? ApplicationName { get; init; }
    /// <summary>
    /// Fully-qualified method name invoked during the audited operation (e.g., <c>UserService.GetAsync</c>).
    /// </summary>
    public string? ActionName { get; init; }
    /// <summary>
    /// Unique identifier of the authenticated user who triggered the operation.
    /// </summary>
    public Guid? UserId { get; init; }
    /// <summary>
    /// Login name of the user who performed the operation.
    /// </summary>
    public string? UserName { get; init; }
    /// <summary>
    /// Unique identifier of the tenant scope in which the operation was executed. <c>null</c> for host-level operations.
    /// </summary>
    public Guid? TenantId { get; init; }
    /// <summary>
    /// Display name of the tenant.
    /// </summary>
    public string? TenantName { get; init; }
    /// <summary>
    /// Unique identifier of the impersonating user, if the operation was performed under impersonation.
    /// </summary>
    public Guid? ImpersonatorUserId { get; init; }
    /// <summary>
    /// Unique identifier of the impersonator's tenant, if applicable.
    /// </summary>
    public Guid? ImpersonatorTenantId { get; init; }
    /// <summary>
    /// UTC timestamp when the operation was executed.
    /// </summary>
    public DateTimeOffset ExecutionTime { get; init; }
    /// <summary>
    /// Total execution time in milliseconds.
    /// </summary>
    public int ExecutionDuration { get; init; }
    /// <summary>
    /// OAuth2 client identifier used to make the request.
    /// </summary>
    public string? ClientId { get; init; }
    /// <summary>
    /// Distributed tracing correlation ID for linking related operations across services.
    /// </summary>
    public string? CorrelationId { get; init; }
    /// <summary>
    /// IP address of the client that initiated the request.
    /// </summary>
    public string? ClientIpAddress { get; init; }
    /// <summary>
    /// HTTP method of the request (e.g., <c>GET</c>, <c>POST</c>, <c>PUT</c>, <c>DELETE</c>).
    /// </summary>
    public string? HttpMethod { get; init; }
    /// <summary>
    /// HTTP status code returned by the response.
    /// </summary>
    public int? HttpStatusCode { get; init; }
    /// <summary>
    /// Classification of the transport channel that produced this audit record.
    /// </summary>
    public CRequestType RequestType { get; init; }
    /// <summary>
    /// Request URL path that triggered the operation.
    /// </summary>
    public string? Url { get; init; }
    /// <summary>
    /// User-Agent header or client browser information.
    /// </summary>
    public string? BrowserInfo { get; init; }
    /// <summary>
    /// Exception details if the operation resulted in an error.
    /// </summary>
    public string? Exceptions { get; init; }
    /// <summary>
    /// Free-text comment attached to the audit entry.
    /// </summary>
    public string? Comments { get; init; }
    /// <summary>
    /// Serialized JSON bag of additional properties captured during the operation.
    /// </summary>
    public string? ExtraProperties { get; init; }

    /// <summary>
    /// Collection of method invocations recorded within this audit entry.
    /// </summary>
    public List<AuditLogActionDto> Actions { get; init; } = [];
    /// <summary>
    /// Collection of entity-level changes (create, update, delete) tracked within this audit entry.
    /// </summary>
    public List<EntityChangeDto> EntityChanges { get; init; } = [];

    /// <summary>
    /// Service name extracted from the first recorded action.
    /// </summary>
    public string? ServiceName => Actions.FirstOrDefault()?.ServiceName;
    /// <summary>
    /// Method name extracted from the first recorded action.
    /// </summary>
    public string? MethodName => Actions.FirstOrDefault()?.MethodName;
}

/// <summary>
/// DTO for a single audit log action (method invocation).
/// </summary>
public sealed class AuditLogActionDto : EntityDto<Guid>
{
    /// <summary>
    /// Parent audit log identifier.
    /// </summary>
    public Guid AuditLogId { get; init; }
    /// <summary>
    /// Fully-qualified service or class name containing the invoked method.
    /// </summary>
    public string ServiceName { get; init; } = null!;
    /// <summary>
    /// Name of the method that was invoked.
    /// </summary>
    public string MethodName { get; init; } = null!;
    /// <summary>
    /// Serialized method arguments at the time of invocation.
    /// </summary>
    public string? Parameters { get; init; }
    /// <summary>
    /// UTC timestamp when the method was invoked.
    /// </summary>
    public DateTimeOffset ExecutionTime { get; init; }
    /// <summary>
    /// Execution time of the method invocation in milliseconds.
    /// </summary>
    public int ExecutionDuration { get; init; }
}

/// <summary>
/// DTO for a single entity change within an audit log.
/// </summary>
public sealed class EntityChangeDto : EntityDto<Guid>
{
    /// <summary>
    /// Parent audit log identifier.
    /// </summary>
    public Guid AuditLogId { get; init; }
    /// <summary>
    /// UTC timestamp when the entity change occurred.
    /// </summary>
    public DateTimeOffset ChangeTime { get; init; }
    /// <summary>
    /// Type of change (<see cref="CEntityChangeType"/>).
    /// </summary>
    public CEntityChangeType ChangeType { get; init; }
    /// <summary>
    /// Tenant that owns the changed entity.
    /// </summary>
    public Guid? EntityTenantId { get; init; }
    /// <summary>
    /// Primary key of the changed entity (stringified).
    /// </summary>
    public string? EntityId { get; init; }
    /// <summary>
    /// Fully-qualified CLR type name of the changed entity.
    /// </summary>
    public string? EntityTypeFullName { get; init; }

    /// <summary>
    /// Collection of individual property-level changes within this entity change.
    /// </summary>
    public List<EntityPropertyChangeDto> PropertyChanges { get; init; } = [];
}

/// <summary>
/// DTO for a single property change within an entity change.
/// </summary>
public sealed class EntityPropertyChangeDto : EntityDto<Guid>
{
    /// <summary>
    /// Parent entity change identifier.
    /// </summary>
    public Guid EntityChangeId { get; init; }
    /// <summary>
    /// Name of the property that changed.
    /// </summary>
    public string PropertyName { get; init; } = null!;
    /// <summary>
    /// Fully-qualified CLR type name of the property.
    /// </summary>
    public string PropertyTypeFullName { get; init; } = null!;
    /// <summary>
    /// Value before the change (serialized as string).
    /// </summary>
    public string? OriginalValue { get; init; }
    /// <summary>
    /// Value after the change (serialized as string).
    /// </summary>
    public string? NewValue { get; init; }
}
