using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace LHA.BlazorWasm.Components.Json;

public partial class JsonViewer : LHAComponentBase
{
    [Parameter] public string? Value { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public string MaxHeight { get; set; } = "500px";
    [Parameter] public bool AllowCopy { get; set; } = true;
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private string FormatJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        try
        {
            if (json.TrimStart().StartsWith('{') || json.TrimStart().StartsWith('['))
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, _jsonOptions);
            }
            return json;
        }
        catch
        {
            return json;
        }
    }
}
