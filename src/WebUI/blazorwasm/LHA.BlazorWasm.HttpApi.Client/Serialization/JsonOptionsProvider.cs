using System.Text.Json;
using System.Text.Json.Serialization;

namespace LHA.BlazorWasm.HttpApi.Client.Serialization;

/// <summary>
/// Provides centralized JSON serialization options for the API client.
/// </summary>
public static class JsonOptionsProvider
{
    public static JsonSerializerOptions Default { get; }

    static JsonOptionsProvider()
    {
        Default = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
        
        Default.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }
}
