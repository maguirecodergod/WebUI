namespace LHA.Auditing;

/// <summary>
/// Type of entity change tracked in audit logs.
/// </summary>
public enum EntityChangeType : byte
{
    /// <summary>Entity was created.</summary>
    Created = 0,

    /// <summary>Entity was updated.</summary>
    Updated = 1,

    /// <summary>Entity was deleted (soft or hard).</summary>
    Deleted = 2
}
