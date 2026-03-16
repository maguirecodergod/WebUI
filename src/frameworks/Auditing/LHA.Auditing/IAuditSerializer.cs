namespace LHA.Auditing;

/// <summary>
/// Serializes objects for audit log parameter and property change recording.
/// </summary>
public interface IAuditSerializer
{
    /// <summary>Serializes the given object to a string representation.</summary>
    string Serialize(object obj);
}
