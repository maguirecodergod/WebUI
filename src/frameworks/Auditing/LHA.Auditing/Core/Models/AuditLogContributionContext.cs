namespace LHA.Auditing;

/// <summary>
/// Context passed to <see cref="IAuditLogContributor"/> callbacks.
/// Provides access to the scoped service provider and the audit log being built.
/// </summary>
public sealed class AuditLogContributionContext
{
    /// <summary>Scoped service provider for resolving dependencies.</summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>The audit log entry being built.</summary>
    public AuditLogEntry AuditLog { get; }

    public AuditLogContributionContext(IServiceProvider serviceProvider, AuditLogEntry auditLog)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(auditLog);
        ServiceProvider = serviceProvider;
        AuditLog = auditLog;
    }
}
