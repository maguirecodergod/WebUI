namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Represents a standard error model returned by the API.
/// </summary>
public record ApiError
{
    /// <summary>
    /// The error code.
    /// </summary>
    public string? Code { get; init; }
    /// <summary>
    /// The error message.
    /// </summary>
    public string? Message { get; init; }
    /// <summary>
    /// The error details.
    /// </summary>
    public string? Details { get; init; }
    /// <summary>
    /// The validation errors.
    /// </summary>
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
