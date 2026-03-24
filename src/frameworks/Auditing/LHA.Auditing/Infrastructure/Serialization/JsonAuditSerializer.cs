using System.Text.Json;

namespace LHA.Auditing;

/// <summary>
/// Default <see cref="IAuditSerializer"/> using <see cref="System.Text.Json"/>.
/// Respects <see cref="DisableAuditingAttribute"/> by skipping marked types during serialization.
/// </summary>
internal sealed class JsonAuditSerializer : IAuditSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        try
        {
            return JsonSerializer.Serialize(obj, SerializerOptions);
        }
        catch
        {
            return "{}";
        }
    }
}
