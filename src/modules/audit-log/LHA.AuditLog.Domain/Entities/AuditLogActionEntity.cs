using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.AuditLog.Domain;

/// <summary>
/// Represents a single audit log action (method invocation) within an <see cref="AuditLogEntity"/>.
/// </summary>
public sealed class AuditLogActionEntity : Entity<Guid>, IMultiTenant
{
    /// <summary>FK to the parent audit log.</summary>
    public Guid AuditLogId { get; private init; }

    /// <summary>Tenant identifier (denormalized for filtering).</summary>
    public Guid? TenantId { get; private init; }

    /// <summary>Fully-qualified service/class name.</summary>
    public string ServiceName { get; private init; } = null!;

    /// <summary>Method name that was executed.</summary>
    public string MethodName { get; private init; } = null!;

    /// <summary>Serialized method parameters.</summary>
    public string? Parameters { get; private init; }

    /// <summary>UTC time the action started.</summary>
    public DateTimeOffset ExecutionTime { get; private init; }

    /// <summary>Duration in milliseconds.</summary>
    public int ExecutionDuration { get; private init; }

    /// <summary>EF Core constructor.</summary>
    private AuditLogActionEntity() { }

    public AuditLogActionEntity(
        Guid auditLogId,
        Guid? tenantId,
        string serviceName,
        string methodName,
        string? parameters,
        DateTimeOffset executionTime,
        int executionDuration)
    {
        Id = Guid.CreateVersion7();
        AuditLogId = auditLogId;
        TenantId = tenantId;
        ServiceName = serviceName;
        MethodName = methodName;
        Parameters = parameters;
        ExecutionTime = executionTime;
        ExecutionDuration = executionDuration;
    }
}
