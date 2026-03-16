namespace LHA.BackgroundJob;

/// <summary>
/// Serializes and deserializes background job arguments.
/// </summary>
public interface IBackgroundJobSerializer
{
    /// <summary>Serializes job arguments to a string.</summary>
    string Serialize(object obj);

    /// <summary>Deserializes job arguments from a string.</summary>
    object Deserialize(string value, Type type);
}
