using System.Text.Json;

namespace LHA.MessageBroker;

/// <summary>
/// Default JSON message serializer using System.Text.Json.
/// Thread-safe, high-performance. Used as the default serializer by both Kafka and RabbitMQ.
/// </summary>
public sealed class SystemTextJsonMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <inheritdoc />
    public string ContentType => "application/json";

    /// <inheritdoc />
    public byte[] Serialize<T>(T value) where T : class
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, Options);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(byte[] data) where T : class
    {
        return JsonSerializer.Deserialize<T>(data, Options);
    }

    /// <inheritdoc />
    public object? Deserialize(byte[] data, Type type)
    {
        return JsonSerializer.Deserialize(data, type, Options);
    }
}
