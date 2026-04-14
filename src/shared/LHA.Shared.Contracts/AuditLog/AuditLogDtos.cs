using LHA.Auditing;
using LHA.Ddd.Application;

namespace LHA.Shared.Contracts.AuditLog;

// ─── AuditLog DTOs ───────────────────────────────────────────────

/// <summary>
/// DTO for a single audit log entry.
/// </summary>
public sealed class AuditLogDto : EntityDto<Guid>
{
    public string? ApplicationName { get; init; }
    public string? ActionName { get; init; }
    public Guid? UserId { get; init; }
    public string? UserName { get; init; }
    public Guid? TenantId { get; init; }
    public string? TenantName { get; init; }
    public Guid? ImpersonatorUserId { get; init; }
    public Guid? ImpersonatorTenantId { get; init; }
    public DateTimeOffset ExecutionTime { get; init; }
    public int ExecutionDuration { get; init; }
    public string? ClientId { get; init; }
    public string? CorrelationId { get; init; }
    public string? ClientIpAddress { get; init; }
    public string? HttpMethod { get; init; }
    public int? HttpStatusCode { get; init; }
    public string? Url { get; init; }
    public string? BrowserInfo { get; init; }
    public string? Exceptions { get; init; }
    public string? Comments { get; init; }
    public string? ExtraProperties { get; init; }

    public List<AuditLogActionDto> Actions { get; init; } = [];
    public List<EntityChangeDto> EntityChanges { get; init; } = [];

    public string? ServiceName => Actions.FirstOrDefault()?.ServiceName;
    public string? MethodName => Actions.FirstOrDefault()?.MethodName;
}

/// <summary>
/// DTO for a single audit log action (method invocation).
/// </summary>
public sealed class AuditLogActionDto : EntityDto<Guid>
{
    public Guid AuditLogId { get; init; }
    public string ServiceName { get; init; } = null!;
    public string MethodName { get; init; } = null!;
    public string? Parameters { get; init; }
    public DateTimeOffset ExecutionTime { get; init; }
    public int ExecutionDuration { get; init; }
}

/// <summary>
/// DTO for a single entity change within an audit log.
/// </summary>
public sealed class EntityChangeDto : EntityDto<Guid>
{
    public Guid AuditLogId { get; init; }
    public DateTimeOffset ChangeTime { get; init; }
    public CEntityChangeType ChangeType { get; init; }
    public Guid? EntityTenantId { get; init; }
    public string? EntityId { get; init; }
    public string? EntityTypeFullName { get; init; }

    public List<EntityPropertyChangeDto> PropertyChanges { get; init; } = [];
}

/// <summary>
/// DTO for a single property change within an entity change.
/// </summary>
public sealed class EntityPropertyChangeDto : EntityDto<Guid>
{
    public Guid EntityChangeId { get; init; }
    public string PropertyName { get; init; } = null!;
    public string PropertyTypeFullName { get; init; } = null!;
    public string? OriginalValue { get; init; }
    public string? NewValue { get; init; }
}
