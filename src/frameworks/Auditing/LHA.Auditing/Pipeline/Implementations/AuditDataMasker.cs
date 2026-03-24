using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Pipeline;

/// <summary>
/// Default <see cref="IAuditDataMasker"/> that masks sensitive JSON fields
/// based on <see cref="AuditPipelineOptions.SensitiveFieldNames"/>.
/// <para>
/// Uses <see cref="JsonNode"/> for lightweight DOM traversal without deserialization
/// to concrete types. Handles nested objects and arrays recursively.
/// </para>
/// </summary>
internal sealed class AuditDataMasker : IAuditDataMasker
{
    private const string DefaultMask = "****";
    private readonly HashSet<string> _sensitiveFields;

    public AuditDataMasker(IOptions<AuditPipelineOptions> options)
    {
        _sensitiveFields = options.Value.SensitiveFieldNames;
    }

    /// <inheritdoc />
    public string? MaskJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;

        try
        {
            var node = JsonNode.Parse(json);
            if (node is null)
                return json;

            MaskNode(node);
            return node.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch
        {
            // Not valid JSON — return as-is
            return json;
        }
    }

    private void MaskNode(JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                var keysToMask = new List<string>();

                foreach (var kvp in obj)
                {
                    if (_sensitiveFields.Contains(kvp.Key))
                    {
                        keysToMask.Add(kvp.Key);
                    }
                    else if (kvp.Value is not null)
                    {
                        MaskNode(kvp.Value);
                    }
                }

                foreach (var key in keysToMask)
                {
                    obj[key] = DefaultMask;
                }
                break;

            case JsonArray array:
                foreach (var item in array)
                {
                    if (item is not null)
                        MaskNode(item);
                }
                break;
        }
    }
}
