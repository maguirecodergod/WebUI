using System.Security.Claims;
using System.Text.Json;
using LHA.BlazorWasm.Services.Storage;
using LHA.Shared.Contracts.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace LHA.BlazorWasm.Services.Auth;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IPermissionService _permissionService;
    private readonly LHA.BlazorWasm.HttpApi.Client.Abstractions.IClientContextProvider _clientContextProvider;
    private readonly AuthTokenCache _authTokenCache;

    // Use a constant key for storing AuthResultDto. You can define this in your shared constants.
    private const string AuthStorageKey = "auth_result";
    private const string TenantIdStorageKey = "current_tenant_id";

    public ApiAuthenticationStateProvider(
        ILocalStorageService localStorage,
        IPermissionService permissionService,
        LHA.BlazorWasm.HttpApi.Client.Abstractions.IClientContextProvider clientContextProvider,
        AuthTokenCache authTokenCache)
    {
        _localStorage = localStorage;
        _permissionService = permissionService;
        _clientContextProvider = clientContextProvider;
        _authTokenCache = authTokenCache;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);

            if (authResult == null || string.IsNullOrWhiteSpace(authResult.AccessToken))
            {
                _authTokenCache.Clear();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _authTokenCache.SetToken(authResult.AccessToken);

            var claims = ParseClaimsFromJwt(authResult.AccessToken);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Persist TenantId if found in JWT
            var tenantId = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantId))
            {
                await _localStorage.SetAsync(TenantIdStorageKey, tenantId);
                (_clientContextProvider as PersistentClientContextProvider)?.SetTenantId(tenantId);
            }

            _permissionService.SetUser(user);
            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticatedAsync(AuthResultDto authResult)
    {
        await _localStorage.SetAsync(AuthStorageKey, authResult);
        _authTokenCache.SetToken(authResult.AccessToken);

        var claims = ParseClaimsFromJwt(authResult.AccessToken!);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        // Persist TenantId if found in JWT
        var tenantId = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await _localStorage.SetAsync(TenantIdStorageKey, tenantId);
            (_clientContextProvider as PersistentClientContextProvider)?.SetTenantId(tenantId);
        }
        else
        {
            await _localStorage.RemoveAsync(TenantIdStorageKey);
            (_clientContextProvider as PersistentClientContextProvider)?.SetTenantId(null);
        }

        _permissionService.SetUser(user);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveAsync(AuthStorageKey);
        await _localStorage.RemoveAsync(TenantIdStorageKey);
        _authTokenCache.Clear();
        (_clientContextProvider as PersistentClientContextProvider)?.SetTenantId(null);
        _permissionService.SetUser(null);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                        {
                            claims.Add(new Claim(kvp.Key, item.ToString() ?? string.Empty));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, element.ToString() ?? string.Empty));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty));
                }
            }
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
