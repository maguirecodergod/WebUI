namespace LHA.MessageBroker;

/// <summary>
/// Abstraction for message serialization/deserialization.
/// Allows swapping between JSON, Protobuf, Avro, MessagePack, etc.
/// Implementations must be thread-safe.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>Serializes an object to a byte array.</summary>
    byte[] Serialize<T>(T value) where T : class;

    /// <summary>Deserializes a byte array to the specified type.</summary>
    T? Deserialize<T>(byte[] data) where T : class;

    /// <summary>Deserializes a byte array to the specified type.</summary>
    object? Deserialize(byte[] data, Type type);

    /// <summary>The MIME content type produced by this serializer (e.g. "application/json").</summary>
    string ContentType { get; }
}
