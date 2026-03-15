using System.Net;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Standard wrapper for API responses.
/// </summary>
/// <typeparam name="T">The type of the expected response body.</typeparam>
public record ApiResponse<T>
{
    public bool Success => StatusCode >= HttpStatusCode.OK && StatusCode < HttpStatusCode.MultipleChoices && Error == null;
    
    public T? Data { get; init; }
    
    public ApiError? Error { get; init; }
    
    public HttpStatusCode StatusCode { get; init; }
}
