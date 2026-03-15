using LHA.BlazorWasm.HttpApi.Client.Core;

namespace LHA.BlazorWasm.HttpApi.Client.Abstractions;

/// <summary>
/// Centralized API error handler responsible for mapping HTTP errors to domain-specific ApiExceptions.
/// </summary>
public interface IApiErrorHandler
{
    /// <summary>
    /// Handles the API error synchronously or asynchronously, throwing an ApiException.
    /// </summary>
    /// <param name="responseMessage">The failed HTTP response message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, usually throwing the mapped exception.</returns>
    Task HandleErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default);
}
