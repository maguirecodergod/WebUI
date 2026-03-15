using System.Net.Http.Json;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Serialization;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// A default implementation for parsing problem+json or raw errors from the API response payload.
/// </summary>
public class DefaultApiErrorHandler : IApiErrorHandler
{
    public virtual async Task HandleErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        var requestPath = responseMessage.RequestMessage?.RequestUri?.ToString();
        string rawResponse = string.Empty;
        ApiError? apiError = null;

        if (responseMessage.Content != null)
        {
            try
            {
                rawResponse = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                var contentType = responseMessage.Content.Headers.ContentType?.MediaType;

                if (!string.IsNullOrWhiteSpace(rawResponse))
                {
                    if (string.Equals(contentType, "application/problem+json", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase))
                    {
                        apiError = await responseMessage.Content.ReadFromJsonAsync<ApiError>(JsonOptionsProvider.Default, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        apiError = new ApiError { Message = rawResponse, Code = responseMessage.StatusCode.ToString() };
                    }
                }
            }
            catch
            {
                // Fallback to raw response if parsing fails
                if (apiError == null) 
                {
                    apiError = new ApiError { Message = rawResponse, Code = "ParseError" };
                }
            }
        }

        throw new ApiException(
            $"API request failed with status code {responseMessage.StatusCode}",
            responseMessage.StatusCode,
            requestPath,
            apiError,
            rawResponse);
    }

    public virtual Task HandleExceptionAsync(Exception exception, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        // Default implementation just rethrows or wraps the exception
        // Usually, logging happens in handlers, so we don't need to do much here
        // if we want to preserve the standard flow.
        return Task.CompletedTask;
    }
}
