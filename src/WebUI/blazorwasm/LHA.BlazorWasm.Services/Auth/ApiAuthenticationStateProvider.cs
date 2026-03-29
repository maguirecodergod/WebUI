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

    // Use a constant key for storing AuthResultDto. You can define this in your shared constants.
    private const string AuthStorageKey = "auth_result";

    public ApiAuthenticationStateProvider(ILocalStorageService localStorage, IPermissionService permissionService)
    {
        _localStorage = localStorage;
        _permissionService = permissionService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);

            if (authResult == null || string.IsNullOrWhiteSpace(authResult.AccessToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(authResult.AccessToken), "jwt");
            var user = new ClaimsPrincipal(identity);

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
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(authResult.AccessToken!), "jwt");
        var user = new ClaimsPrincipal(identity);
        
        _permissionService.SetUser(user);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveAsync(AuthStorageKey);
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
