using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Forms;
using LHA.BlazorWasm.Components.Topbar;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Services.Auth;
using LHA.Shared.Contracts.Identity.Auth;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.App.Pages.Auth
{
    public partial class LoginPage : LHAComponentBase
    {
        [Inject] public AuthApiClient AuthClient { get; set; } = default!;
        [Inject] public ApiAuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] public ITopbarService TopbarService { get; set; } = default!;

        private LoginInput Model { get; set; } = new();
        private ServerSideValidator _serverValidator = default!;
        private bool IsLoading { get; set; } = false;

        [SupplyParameterFromQuery(Name = "returnUrl")]
        public string? ReturnUrl { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ThemeState.OnThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(LHA.BlazorWasm.Services.Theme.CThemeMode mode)
        {
            InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            ThemeState.OnThemeChanged -= OnThemeChanged;
        }

        private async Task HandleLogin()
        {
            if (IsLoading) return;

            IsLoading = true;
            _serverValidator.ClearErrors();
            StateHasChanged();

            try
            {
                var authResult = await AuthClient.LoginAsync(Model);

                if (authResult != null && !string.IsNullOrWhiteSpace(authResult.AccessToken))
                {
                    await AuthStateProvider.MarkUserAsAuthenticatedAsync(authResult);

                    // Load profile and permissions before redirecting
                    await TopbarService.LoadUserProfileAsync();

                    var redirectUrl = string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : Uri.UnescapeDataString(ReturnUrl);
                    Navigation.NavigateTo(redirectUrl);
                }
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest
                    || ex.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity
                    || ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                    || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    if (ex.ApiError?.ValidationErrors != null && ex.ApiError.ValidationErrors.Any())
                    {
                        _serverValidator.DisplayErrors(ex.ApiError.ValidationErrors);
                    }
                    else if (!string.IsNullOrWhiteSpace(ex.ApiError?.Message))
                    {
                        // If it's a general unauthorized/forbidden error without specific field targets, show toast
                        ToastNotification.Error(ex.ApiError.Message);
                    }
                }
            }
            catch (Exception)
            {
                // Transient faults handled centrally by ToastApiErrorHandler
                // But we can add a local generic toast if needed:
                ToastNotification.Error(L("Login.GenericError"));
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private void GoToRegister()
        {
            Navigation.NavigateTo("/register-tenant");
        }
    }
}