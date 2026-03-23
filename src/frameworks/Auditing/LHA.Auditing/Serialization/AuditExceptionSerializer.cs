using System.Text.Json;

namespace LHA.Auditing.Serialization;

/// <summary>
/// Structured exception information for audit log storage.
/// Flattened to avoid deep nesting in serialized output.
/// </summary>
public sealed class AuditExceptionInfo
{
    /// <summary>Exception type name.</summary>
    public string? Type { get; set; }

    /// <summary>Exception message.</summary>
    public string? Message { get; set; }

    /// <summary>Stack trace.</summary>
    public string? StackTrace { get; set; }

    /// <summary>Inner exception (flattened).</summary>
    public AuditExceptionInfo? InnerException { get; set; }

    /// <summary>Additional data dictionary.</summary>
    public Dictionary<string, string>? Data { get; set; }
}

/// <summary>
/// Serializes exceptions into structured, safe JSON for audit logging.
/// </summary>
public static class AuditExceptionSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Converts an exception to a structured JSON string, limiting depth
    /// to prevent stack overflow on deeply nested inner exceptions.
    /// </summary>
    /// <param name="exception">The exception to serialize.</param>
    /// <param name="maxDepth">Maximum inner exception depth. Default: 5.</param>
    public static string Serialize(Exception exception, int maxDepth = 5)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var info = ToExceptionInfo(exception, maxDepth, 0);
        return JsonSerializer.Serialize(info, JsonOptions);
    }

    private static AuditExceptionInfo ToExceptionInfo(Exception exception, int maxDepth, int currentDepth)
    {
        var info = new AuditExceptionInfo
        {
            Type = exception.GetType().FullName,
            Message = exception.Message,
            StackTrace = exception.StackTrace
        };

        // Serialize exception.Data (only string keys/values to avoid serialization issues)
        if (exception.Data.Count > 0)
        {
            info.Data = [];
            foreach (var key in exception.Data.Keys)
            {
                if (key is string strKey)
                {
                    try
                    {
                        info.Data[strKey] = exception.Data[key]?.ToString() ?? "null";
                    }
                    catch
                    {
                        info.Data[strKey] = "<serialization failed>";
                    }
                }
            }
        }

        // Recurse into inner exception with depth limit
        if (exception.InnerException is not null && currentDepth < maxDepth)
        {
            info.InnerException = ToExceptionInfo(exception.InnerException, maxDepth, currentDepth + 1);
        }

        return info;
    }
}
