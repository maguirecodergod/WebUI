using System.Text.Json;

namespace LHA.Caching;

/// <summary>
/// Default serializer that uses <see cref="System.Text.Json"/> for cache item serialization.
/// </summary>
public sealed class JsonDistributedCacheSerializer : IDistributedCacheSerializer
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonDistributedCacheSerializer(JsonSerializerOptions? jsonOptions = null)
    {
        _jsonOptions = jsonOptions ?? JsonSerializerOptions.Default;
    }

    /// <inheritdoc />
    public byte[] Serialize<T>(T value) =>
        JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);

    /// <inheritdoc />
    public T Deserialize<T>(byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes, _jsonOptions)!;
}
