namespace LHA.Auditing;

/// <summary>
/// Disables audit logging for a class, method, or property.
/// Takes precedence over <see cref="AuditedAttribute"/> for excluded members.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
public sealed class DisableAuditingAttribute : Attribute;
