using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Auth;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

/// <summary>
/// Client for authentication and self-service registration endpoints.
/// </summary>
public class AuthApiClient : ApiClientBase
{
    private const string BaseUrl = "api/v1/identity/auth";

    public AuthApiClient(HttpClient httpClient, IApiErrorHandler errorHandler)
        : base(httpClient, errorHandler)
    {
    }

    /// <summary>
    /// Registers a new tenant and an initial admin account.
    /// Returns the parsed AuthResultDto which contains JWT tokens.
    /// </summary>
    public async Task<AuthResultDto?> RegisterTenantAsync(RegisterTenantInput input)
    {
        var response = await PostAsync<RegisterTenantInput, AuthResultDto>($"{BaseUrl}/register-tenant", input);
        return response.Result.Data;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    public async Task<AuthResultDto?> LoginAsync(LoginModel input)
    {
        var response = await PostAsync<LoginModel, AuthResultDto>($"{BaseUrl}/login", input);
        return response.Result.Data;
    }
}
