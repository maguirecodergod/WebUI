using System.Text.Json;
using System.Text.Json.Nodes;

namespace LHA.Auditing;

/// <summary>
/// Masks sensitive JSON property values (e.g. passwords, tokens) with "***".
/// Works on both JSON strings and CLR object dictionaries.
/// </summary>
public static class SensitiveDataMasker
{
    private const string MaskValue = "***";

    // ─── JSON string masking ──────────────────────────────────────────────

    /// <summary>
    /// Parses a JSON string, replaces values of any property whose name is in
    /// <paramref name="sensitiveNames"/> with "***", and returns the result.
    /// Returns the original string unchanged if it is not valid JSON.
    /// </summary>
    public static string MaskJson(string json, IReadOnlySet<string> sensitiveNames)
    {
        if (string.IsNullOrWhiteSpace(json) || sensitiveNames.Count == 0)
            return json;

        try
        {
            var node = JsonNode.Parse(json);
            if (node is null) return json;

            MaskNode(node, sensitiveNames);
            return node.ToJsonString();
        }
        catch
        {
            return json; // not valid JSON – return as-is
        }
    }

    // ─── Dictionary masking ───────────────────────────────────────────────

    /// <summary>
    /// Returns a shallow copy of <paramref name="dict"/> where values of any key
    /// in <paramref name="sensitiveNames"/> are replaced by "***".
    /// Recursively masks nested dictionaries and JSON-string values.
    /// </summary>
    public static Dictionary<string, object?> MaskDictionary(
        Dictionary<string, object?> dict,
        IReadOnlySet<string> sensitiveNames)
    {
        var result = new Dictionary<string, object?>(dict.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in dict)
        {
            if (sensitiveNames.Contains(key))
            {
                result[key] = MaskValue;
                continue;
            }

            result[key] = value switch
            {
                // Nested dictionary
                Dictionary<string, object?> nested => MaskDictionary(nested, sensitiveNames),
                // JSON string that itself may contain sensitive keys
                string s when IsJsonString(s) => MaskJson(s, sensitiveNames),
                _ => value
            };
        }

        return result;
    }

    // ─── Private helpers ──────────────────────────────────────────────────

    private static void MaskNode(JsonNode node, IReadOnlySet<string> sensitiveNames)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var key in obj.Select(kv => kv.Key).ToList())
                {
                    if (sensitiveNames.Contains(key))
                    {
                        obj[key] = JsonValue.Create(MaskValue);
                    }
                    else if (obj[key] is JsonNode child)
                    {
                        MaskNode(child, sensitiveNames);
                    }
                }
                break;

            case JsonArray arr:
                foreach (var item in arr)
                {
                    if (item is not null) MaskNode(item, sensitiveNames);
                }
                break;
        }
    }

    private static bool IsJsonString(string s)
    {
        var trimmed = s.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }
}
