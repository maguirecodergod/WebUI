using System.Net.Http.Json;
using System.Text.Json;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Serialization;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Base class for strongly typed API clients, handling standard HTTP mechanisms.
/// </summary>
public abstract class ApiClientBase : IApiClient
{
    protected readonly HttpClient HttpClient;
    protected readonly IApiErrorHandler ErrorHandler;

    protected ApiClientBase(HttpClient httpClient, IApiErrorHandler errorHandler)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    }

    protected virtual Task<ApiResponse<T>> GetAsync<T>(string uri, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        configureRequest?.Invoke(request);
        return SendAsync<T>(request, cancellationToken);
    }

    protected virtual Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest requestData, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(requestData, options: JsonOptionsProvider.Default)
        };
        configureRequest?.Invoke(request);
        return SendAsync<TResponse>(request, cancellationToken);
    }

    protected virtual Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string uri, TRequest requestData, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(requestData, options: JsonOptionsProvider.Default)
        };
        configureRequest?.Invoke(request);
        return SendAsync<TResponse>(request, cancellationToken);
    }

    protected virtual Task<ApiResponse<T>> DeleteAsync<T>(string uri, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        configureRequest?.Invoke(request);
        return SendAsync<T>(request, cancellationToken);
    }

    protected virtual async Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            return await HandleResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not ApiException)
        {
            await ErrorHandler.HandleExceptionAsync(ex, request, cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    protected virtual async Task<ApiResponse<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            await ErrorHandler.HandleErrorAsync(response, cancellationToken).ConfigureAwait(false);
            
            // Expected to throw in ErrorHandler, but if left unhandled manually fallback:
            return new ApiResponse<T>
            {
                StatusCode = response.StatusCode,
                Error = new ApiError { Message = $"HTTP Error {response.StatusCode}" }
            };
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return new ApiResponse<T>
            {
                StatusCode = response.StatusCode,
                Data = default
            };
        }

        try
        {
            // If the response is meant to be a string
            if (typeof(T) == typeof(string))
            {
                var stringContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return new ApiResponse<T>
                {
                    StatusCode = response.StatusCode,
                    Data = (T)(object)stringContent
                };
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            if (stream.Length == 0)
            {
                 return new ApiResponse<T> { StatusCode = response.StatusCode, Data = default };
            }

            var data = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptionsProvider.Default, cancellationToken).ConfigureAwait(false);
            return new ApiResponse<T>
            {
                StatusCode = response.StatusCode,
                Data = data
            };
        }
        catch (Exception ex)
        {
            throw new ApiException(
                "Failed to deserialize the successful response.",
                response.StatusCode,
                response.RequestMessage?.RequestUri?.ToString(),
                null,
                null,
                ex);
        }
    }
}
