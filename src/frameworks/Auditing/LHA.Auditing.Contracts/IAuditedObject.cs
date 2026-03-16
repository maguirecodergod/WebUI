namespace LHA.Auditing;

/// <summary>
/// Composite interface for entities with creation and modification auditing.
/// </summary>
public interface IAuditedObject : ICreationAuditedObject, IModificationAuditedObject;
