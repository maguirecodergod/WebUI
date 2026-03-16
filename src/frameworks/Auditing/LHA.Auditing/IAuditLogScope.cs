namespace LHA.Auditing;

/// <summary>
/// Provides access to the current <see cref="AuditLogEntry"/> within an audit scope.
/// </summary>
public interface IAuditLogScope
{
    /// <summary>The audit log entry being built in this scope.</summary>
    AuditLogEntry Log { get; }
}
