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
    private readonly AuthTokenCache _cache;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private const string AuthStorageKey = "auth_result";

    public StorageAccessTokenProvider(ILocalStorageService localStorage, AuthTokenCache cache)
    {
        _localStorage = localStorage;
        _cache = cache;
    }

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.IsInitialized)
        {
            return _cache.AccessToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_cache.IsInitialized)
            {
                return _cache.AccessToken;
            }

            var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);
            _cache.SetToken(authResult?.AccessToken);
        }
        catch
        {
            _cache.SetToken(null);
        }
        finally
        {
            _semaphore.Release();
        }

        return _cache.AccessToken;
    }
}
