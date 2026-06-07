using System.Net.Http.Json;
using System.Text.Json;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Serialization;
using LHA.Ddd.Application;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// Base class for strongly typed API clients, handling standard HTTP mechanisms.
/// </summary>
public abstract class ApiClientBase : IApiClient
{
    /// <summary>
    /// The HTTP client used for API requests.
    /// </summary>
    protected readonly HttpClient HttpClient;
    /// <summary>
    /// The error handler used to handle API errors.
    /// </summary>
    protected readonly IApiErrorHandler ErrorHandler;

    /// <summary>
    /// Initializes a new instance of the ApiClientBase class.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="errorHandler"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected ApiClientBase(HttpClient httpClient, IApiErrorHandler errorHandler)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
    }

    /// <summary>
    /// Sends a GET request to the specified URI.
    /// Handles the response and returns it as a strongly typed object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uri"></param>
    /// <param name="configureRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<ApiResponse<T>> GetAsync<T>(string uri, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        configureRequest?.Invoke(request);
        return SendAsync<T>(request, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request to the specified URI.
    /// Handles the response and returns it as a strongly typed object.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="uri"></param>
    /// <param name="requestData"></param>
    /// <param name="configureRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string uri, TRequest requestData, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(requestData, options: JsonOptionsProvider.Default)
        };
        configureRequest?.Invoke(request);
        return SendAsync<TResponse>(request, cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request to the specified URI.
    /// Handles the response and returns it as a strongly typed object.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="uri"></param>
    /// <param name="requestData"></param>
    /// <param name="configureRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(string uri, TRequest requestData, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(requestData, options: JsonOptionsProvider.Default)
        };
        configureRequest?.Invoke(request);
        return SendAsync<TResponse>(request, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI.
    /// Handles the response and returns it as a strongly typed object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uri"></param>
    /// <param name="configureRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<ApiResponse<T>> DeleteAsync<T>(string uri, Action<HttpRequestMessage>? configureRequest = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        configureRequest?.Invoke(request);
        return SendAsync<T>(request, cancellationToken);
    }

    /// <summary>
    /// Sends the specified HTTP request and handles the response.
    /// Returns the response as a strongly typed object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Handles the specified HTTP response and returns it as a strongly typed object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ApiException"></exception>
    protected virtual async Task<ApiResponse<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            await ErrorHandler.HandleErrorAsync(response, cancellationToken).ConfigureAwait(false);
            
            // Expected to throw in ErrorHandler, but if left unhandled manually fallback:
            return new ApiResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                Result = new ResponseResult<T> { Success = false, Errors = [new ErrorDetailDto { Code = "HTTP_ERROR", Message = $"HTTP Error {response.StatusCode}" }] }
            };
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return ApiResponse<T>.Ok(default!, (int)response.StatusCode);
        }

        try
        {
            // If the response is meant to be a string
            if (typeof(T) == typeof(string))
            {
                var stringContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return ApiResponse<T>.Ok((T)(object)stringContent, (int)response.StatusCode);
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            if (stream.Length == 0)
            {
                 return ApiResponse<T>.Ok(default!, (int)response.StatusCode);
            }

            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<T>>(stream, JsonOptionsProvider.Default, cancellationToken).ConfigureAwait(false);
            return apiResponse ?? ApiResponse<T>.Ok(default!, (int)response.StatusCode);
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
