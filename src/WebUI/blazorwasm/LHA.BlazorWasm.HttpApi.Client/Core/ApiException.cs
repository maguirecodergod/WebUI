using System.Net;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Exception representing a failure when making an API call, including HTTP status and problem details.
/// </summary>
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? RequestPath { get; }
    public ApiError? ApiError { get; }
    public string? RawResponse { get; }

    public ApiException(
        string message,
        HttpStatusCode statusCode,
        string? requestPath = null,
        ApiError? apiError = null,
        string? rawResponse = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        RequestPath = requestPath;
        ApiError = apiError;
        RawResponse = rawResponse;
    }
}
