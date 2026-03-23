namespace LHA.Auditing;

/// <summary>
/// Marks a property or parameter to be completely excluded from audit log serialization.
/// <para>
/// Apply to sensitive properties that should never appear in audit logs,
/// even in masked form (e.g., private keys, raw tokens).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
public sealed class AuditIgnoreAttribute : Attribute;
