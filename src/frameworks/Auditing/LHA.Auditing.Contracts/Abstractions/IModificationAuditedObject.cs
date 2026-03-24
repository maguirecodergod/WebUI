namespace LHA.Auditing;

/// <summary>
/// Stores modification audit information: who last modified and when.
/// </summary>
public interface IModificationAuditedObject : IHasModificationTime
{
    /// <summary>Identifier of the user who last modified this entity.</summary>
    Guid? LastModifierId { get; }
}
