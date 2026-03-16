namespace LHA.Auditing;

/// <summary>
/// Manages audit log scopes for tracking operations.
/// Each scope accumulates actions, entity changes, and exceptions
/// into a single <see cref="AuditLogEntry"/>.
/// </summary>
public interface IAuditingManager
{
    /// <summary>
    /// The current audit log scope (<c>null</c> if no scope is active).
    /// </summary>
    IAuditLogScope? Current { get; }

    /// <summary>
    /// Begins a new audit log scope. Dispose the returned handle to save.
    /// Scopes can be nested — the outermost scope owns the save.
    /// </summary>
    IAuditLogSaveHandle BeginScope();
}
