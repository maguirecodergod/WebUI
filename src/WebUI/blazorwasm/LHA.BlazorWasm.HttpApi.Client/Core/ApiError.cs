namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Represents a standard error model returned by the API.
/// </summary>
public record ApiError
{
    public string? Code { get; init; }
    public string? Message { get; init; }
    public string? Details { get; init; }
    public IDictionary<string, string[]>? ValidationErrors { get; init; }
}
