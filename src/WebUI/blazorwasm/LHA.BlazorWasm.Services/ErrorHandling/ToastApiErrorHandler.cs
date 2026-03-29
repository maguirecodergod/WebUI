using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Services.Toast;
using LHA.BlazorWasm.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;

namespace LHA.BlazorWasm.Services.ErrorHandling;

/// <summary>
/// An API error handler that parses errors using the default logic and displays a toast notification,
/// while appropriately routing critical Auth failures (401, 403) exactly like the legacy system behavior.
/// </summary>
public class ToastApiErrorHandler : DefaultApiErrorHandler
{
    private readonly IToastService _toastService;
    private readonly NavigationManager _navManager;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ToastApiErrorHandler(
        IToastService toastService, 
        NavigationManager navManager,
        AuthenticationStateProvider authStateProvider)
    {
        _toastService = toastService;
        _navManager = navManager;
        _authStateProvider = authStateProvider;
    }

    public override async Task HandleErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        var statusCode = (int)responseMessage.StatusCode;

        // Xử lý các lỗi Auth đặc thù theo đúng behaviour cũ: xóa session và redirect.
        // NHƯNG vẫn để nó tiếp tục nổ Exception. Lý do: chặn không code bên dưới chạy tiếp (tránh NullReferenceException),
        // và GlobalErrorBoundary đã được thiết kế để nuốt ngoại lệ Auth này mà không văng màn hình Oops.
        if (statusCode == 401)
        {
            if (_authStateProvider is ApiAuthenticationStateProvider apiAuthStateProvider)
            {
                await apiAuthStateProvider.MarkUserAsLoggedOutAsync();
            }
            
            var relativeUri = _navManager.ToBaseRelativePath(_navManager.Uri);
            var loginUri = string.IsNullOrWhiteSpace(relativeUri) || relativeUri.StartsWith("login", StringComparison.OrdinalIgnoreCase)
                ? "/login"
                : $"/login?returnUrl={Uri.EscapeDataString(relativeUri)}";
                
            _navManager.NavigateTo(loginUri);
        }
        else if (statusCode == 403)
        {
            _navManager.NavigateTo("/forbidden");
        }

        try
        {
            // Call the base logic to parse the error and throw ApiException
            await base.HandleErrorAsync(responseMessage, cancellationToken);
        }
        catch (ApiException ex)
        {
            // Only show toast if it's not a validation error (400), unprocessable entity (422),
            // and NOT an auth error (401, 403) that is already being redirected.
            if (statusCode != 400 && statusCode != 422 && statusCode != 401 && statusCode != 403)
            {
                var message = ex.ApiError?.Message ?? ex.Message;
                _toastService.Error(
                    $"Request to {responseMessage.RequestMessage?.RequestUri?.AbsolutePath} failed.\n{message}", 
                    $"API Error ({statusCode})");
            }

            // Re-throw the exception so the caller can still handle it, e.g. mapping validation errors
            // or letting GlobalErrorBoundary gracefully ignore auth navigations.
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
