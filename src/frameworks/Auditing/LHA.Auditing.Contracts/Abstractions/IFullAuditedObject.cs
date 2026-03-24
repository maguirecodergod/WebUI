namespace LHA.Auditing;

/// <summary>
/// Full audit trail: creation + modification + deletion (soft-delete).
/// The most comprehensive audit contract for entities.
/// </summary>
public interface IFullAuditedObject : IAuditedObject, IDeletionAuditedObject;
