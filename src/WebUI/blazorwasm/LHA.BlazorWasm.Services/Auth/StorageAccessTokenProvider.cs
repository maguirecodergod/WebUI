using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.Services.Storage;
using LHA.Shared.Contracts.Identity;

namespace LHA.BlazorWasm.Services.Auth;

/// <summary>
/// Implementation of IAccessTokenProvider that retrieves the token from LocalStorage.
/// </summary>
public class StorageAccessTokenProvider : IAccessTokenProvider
{
    private readonly ILocalStorageService _localStorage;
    private const string AuthStorageKey = "auth_result";

    public StorageAccessTokenProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);
            return authResult?.AccessToken;
        }
        catch
        {
            return null;
        }
    }
}
