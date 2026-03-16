namespace LHA.Auditing;

/// <summary>
/// Handle returned by <see cref="IAuditingManager.BeginScope"/>.
/// Disposing saves the audit log and closes the scope.
/// </summary>
public interface IAuditLogSaveHandle : IAsyncDisposable
{
    /// <summary>Persists the audit log entry to the configured <see cref="IAuditingStore"/>.</summary>
    Task SaveAsync(CancellationToken cancellationToken = default);
}
