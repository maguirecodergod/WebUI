namespace LHA.Auditing;

/// <summary>
/// Represents a single audit log action (method invocation) within an <see cref="AuditLogEntry"/>.
/// </summary>
public sealed class AuditLogAction
{
    /// <summary>Fully-qualified service/class name.</summary>
    public required string ServiceName { get; init; }

    /// <summary>Method name that was executed.</summary>
    public required string MethodName { get; init; }

    /// <summary>Serialized method parameters.</summary>
    public string? Parameters { get; init; }

    /// <summary>UTC time the action started.</summary>
    public required DateTimeOffset ExecutionTime { get; init; }

    /// <summary>Duration in milliseconds.</summary>
    public int ExecutionDuration { get; set; }
}
