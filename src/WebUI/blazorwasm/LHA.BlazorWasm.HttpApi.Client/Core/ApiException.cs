using System.Net;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Exception representing a failure when making an API call, including HTTP status and problem details.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
    /// <summary>
    /// The request path.
    /// </summary>
    public string? RequestPath { get; }
    /// <summary>
    /// The API error.
    /// </summary>
    public ApiError? ApiError { get; }
    /// <summary>
    /// The raw response.
    /// </summary>
    public string? RawResponse { get; }

    /// <summary>
    /// Initializes a new instance of the ApiException class.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="statusCode"></param>
    /// <param name="requestPath"></param>
    /// <param name="apiError"></param>
    /// <param name="rawResponse"></param>
    /// <param name="innerException"></param>
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
