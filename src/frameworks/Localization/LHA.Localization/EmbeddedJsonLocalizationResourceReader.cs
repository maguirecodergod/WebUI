using System.Reflection;
using System.Text.Json;
using LHA.Localization.Abstraction;

namespace LHA.Localization;

/// <summary>
/// Reads localization key-value pairs from embedded JSON resource files.
/// Expects files named "{culture}.json" (e.g. "en.json", "vi.json") embedded
/// in the assembly under the resource base path namespace.
/// </summary>
public sealed class EmbeddedJsonLocalizationResourceReader : ILocalizationResourceReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string>? ReadStrings(
        Assembly assembly,
        string resourceBasePath,
        string cultureName)
    {
        var resourceName = $"{resourceBasePath}.{cultureName}.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        using var document = JsonDocument.Parse(stream, new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenJsonElement(document.RootElement, result, prefix: null);
        return result;
    }

    /// <summary>
    /// Recursively flattens a JSON object into a string dictionary.
    /// Nested objects use ":" as separator (e.g. "Validation:Required").
    /// </summary>
    private static void FlattenJsonElement(
        JsonElement element,
        Dictionary<string, string> result,
        string? prefix)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var key = prefix is null ? property.Name : $"{prefix}:{property.Name}";
                    FlattenJsonElement(property.Value, result, key);
                }
                break;

            case JsonValueKind.String:
                if (prefix is not null)
                    result[prefix] = element.GetString()!;
                break;

            default:
                if (prefix is not null)
                    result[prefix] = element.ToString();
                break;
        }
    }
}
