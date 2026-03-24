namespace LHA.Auditing;

/// <summary>
/// Stores creation audit information: who created and when.
/// </summary>
public interface ICreationAuditedObject : IHasCreationTime, IMayHaveCreator;
