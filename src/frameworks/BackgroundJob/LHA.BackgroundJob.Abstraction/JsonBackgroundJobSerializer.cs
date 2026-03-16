using System.Text.Json;

namespace LHA.BackgroundJob;

/// <summary>
/// System.Text.Json-based implementation of <see cref="IBackgroundJobSerializer"/>.
/// </summary>
public sealed class JsonBackgroundJobSerializer : IBackgroundJobSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private readonly JsonSerializerOptions _options;

    public JsonBackgroundJobSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? DefaultOptions;
    }

    /// <inheritdoc />
    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, obj.GetType(), _options);
    }

    /// <inheritdoc />
    public object Deserialize(string value, Type type)
    {
        return JsonSerializer.Deserialize(value, type, _options)
            ?? throw new InvalidOperationException(
                $"Deserialization returned null for type {type.FullName}.");
    }
}
