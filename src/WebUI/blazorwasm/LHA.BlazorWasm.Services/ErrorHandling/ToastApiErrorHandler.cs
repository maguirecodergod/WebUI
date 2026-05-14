using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Services.Toast;
using LHA.BlazorWasm.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

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

        if (statusCode == 401)
        {
            if (_authStateProvider is ApiAuthenticationStateProvider apiAuthStateProvider)
            {
                await apiAuthStateProvider.MarkUserAsLoggedOutAsync();
            }

            var uri = new Uri(_navManager.Uri);
            var path = uri.AbsolutePath;

            if (path.EndsWith("/login", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var relativeUri = _navManager.ToBaseRelativePath(_navManager.Uri);
            var loginUri = $"/login?returnUrl={Uri.EscapeDataString("/" + relativeUri)}";

            _navManager.NavigateTo(loginUri);
            return;
        }
        else if (statusCode == 403)
        {
            _navManager.NavigateTo("/forbidden");
            return;
        }

        try
        {
            await base.HandleErrorAsync(responseMessage, cancellationToken);
        }
        catch (ApiException ex)
        {
            if (statusCode != 400 && statusCode != 422 && statusCode != 401 && statusCode != 403)
            {
                var message = ex.ApiError?.Message ?? ex.Message;
                _toastService.Error(
                    $"Request to {responseMessage.RequestMessage?.RequestUri?.AbsolutePath} failed.\n{message}",
                    $"API Error ({statusCode})");
            }
            throw;
        }
    }

    public override Task HandleExceptionAsync(Exception exception, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        _toastService.Error(
            $"Network Error: {exception.Message}",
            "Connection Failed");

        return Task.CompletedTask;
    }
}
