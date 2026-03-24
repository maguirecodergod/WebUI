namespace LHA.Auditing;

/// <summary>
/// Stores deletion audit information: who deleted and when.
/// Implies soft-delete support via <see cref="IHasDeletionTime"/>.
/// </summary>
public interface IDeletionAuditedObject : IHasDeletionTime
{
    /// <summary>Identifier of the user who deleted this entity.</summary>
    Guid? DeleterId { get; }
}
