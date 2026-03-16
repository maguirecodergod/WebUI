namespace LHA.Auditing;

/// <summary>
/// Marks a class, method, or property for audit logging.
/// When applied to a class, all public methods are audited.
/// When applied to a property, changes to that property trigger entity history recording.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
public sealed class AuditedAttribute : Attribute;
