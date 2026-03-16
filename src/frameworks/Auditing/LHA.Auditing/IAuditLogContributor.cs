namespace LHA.Auditing;

/// <summary>
/// Extension point for enriching audit log entries before and after the operation.
/// <para>
/// Register implementations in DI as <see cref="IAuditLogContributor"/> to
/// add custom metadata (e.g., request headers, feature flags, deployment info).
/// </para>
/// </summary>
public interface IAuditLogContributor
{
    /// <summary>
    /// Called when the audit scope begins. Use to add initial metadata.
    /// </summary>
    void PreContribute(AuditLogContributionContext context) { }

    /// <summary>
    /// Called before the audit log is saved. Use to add final metadata.
    /// </summary>
    void PostContribute(AuditLogContributionContext context) { }
}
