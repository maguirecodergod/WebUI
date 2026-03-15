using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Services.Toast;
using System.Net.Http;

namespace LHA.BlazorWasm.Services.ErrorHandling;

/// <summary>
/// An API error handler that parses errors using the default logic and displays a toast notification.
/// </summary>
public class ToastApiErrorHandler : DefaultApiErrorHandler
{
    private readonly IToastService _toastService;

    public ToastApiErrorHandler(IToastService toastService)
    {
        _toastService = toastService;
    }

    public override async Task HandleErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            // Call the base logic to parse the error and throw ApiException
            await base.HandleErrorAsync(responseMessage, cancellationToken);
        }
        catch (ApiException ex)
        {
            // Show toast notification for the API error
            var message = ex.ApiError?.Message ?? ex.Message;
            var statusCode = (int)ex.StatusCode;
            
            _toastService.Error(
                $"Request to {responseMessage.RequestMessage?.RequestUri?.AbsolutePath} failed.\n{message}", 
                $"API Error ({statusCode})");

            // Re-throw the exception so the caller can still handle it if needed
            throw;
        }
    }

    public override Task HandleExceptionAsync(Exception exception, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        // Handle network errors, timeouts, and other non-HTTP-status exceptions
        _toastService.Error(
            $"Network Error: {exception.Message}", 
            "Connection Failed");
            
        return Task.CompletedTask;
    }
}
