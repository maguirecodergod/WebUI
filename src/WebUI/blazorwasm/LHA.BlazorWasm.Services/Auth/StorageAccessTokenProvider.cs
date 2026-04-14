using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.Services.Storage;
using LHA.Shared.Contracts.Identity;
using LHA.Shared.Contracts.Identity.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace LHA.BlazorWasm.Services.Auth;

/// <summary>
/// Implementation of IAccessTokenProvider that retrieves the token from LocalStorage
/// and handles automatic token refreshing.
/// </summary>
public class StorageAccessTokenProvider : IAccessTokenProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthTokenCache _cache;
    private readonly IServiceProvider _serviceProvider;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private const string AuthStorageKey = "auth_result";
    private const int RefreshThresholdSeconds = 60;

    public StorageAccessTokenProvider(
        ILocalStorageService localStorage,
        AuthTokenCache cache,
        IServiceProvider serviceProvider)
    {
        _localStorage = localStorage;
        _cache = cache;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        // 1. Check if we need to refresh or initialize
        bool needsInitialization = !_cache.IsInitialized;
        bool needsRefresh = _cache.IsInitialized &&
                            _cache.ExpiresAt.HasValue &&
                            _cache.ExpiresAt.Value <= DateTimeOffset.UtcNow.AddSeconds(RefreshThresholdSeconds);

        if (!needsInitialization && !needsRefresh)
        {
            return _cache.AccessToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Re-check after acquiring semaphore
            if (_cache.IsInitialized &&
                (!_cache.ExpiresAt.HasValue || _cache.ExpiresAt.Value > DateTimeOffset.UtcNow.AddSeconds(RefreshThresholdSeconds)))
            {
                return _cache.AccessToken;
            }

            // 2. Try to refresh if we have a refresh token
            if (_cache.IsInitialized && !string.IsNullOrEmpty(_cache.RefreshToken))
            {
                var refreshed = await TryRefreshTokenAsync(_cache.RefreshToken);
                if (refreshed)
                {
                    return _cache.AccessToken;
                }
            }

            // 3. If not initialized or refresh failed, try to load from storage
            var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);
            if (authResult != null)
            {
                _cache.SetToken(authResult.AccessToken, authResult.RefreshToken, authResult.ExpiresIn);

                // If the loaded token is also expired, try to refresh it
                if (_cache.ExpiresAt.HasValue && _cache.ExpiresAt.Value <= DateTimeOffset.UtcNow.AddSeconds(RefreshThresholdSeconds))
                {
                    if (!string.IsNullOrEmpty(_cache.RefreshToken))
                    {
                        await TryRefreshTokenAsync(_cache.RefreshToken);
                    }
                }
            }
            else
            {
                _cache.Clear();
            }
        }
        catch
        {
            _cache.Clear();
        }
        finally
        {
            _semaphore.Release();
        }

        return _cache.AccessToken;
    }

    public async ValueTask<string?> ForceRefreshAsync(string? failedToken = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // If the failed token is provided, and the current token is already different,
            // it means another concurrent request already refreshed the token successfully.
            if (failedToken != null && _cache.IsInitialized && !string.IsNullOrEmpty(_cache.AccessToken) && _cache.AccessToken != failedToken)
            {
                return _cache.AccessToken;
            }

            if (string.IsNullOrEmpty(_cache.RefreshToken))
            {
                // Try loading from storage if cache is empty
                var authResult = await _localStorage.GetAsync<AuthResultDto>(AuthStorageKey);
                if (authResult != null && !string.IsNullOrEmpty(authResult.RefreshToken))
                {
                    await TryRefreshTokenAsync(authResult.RefreshToken);
                }
            }
            else
            {
                await TryRefreshTokenAsync(_cache.RefreshToken);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return _cache.AccessToken;
    }

    private async Task<bool> TryRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var factory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
            using var httpClient = factory.CreateClient(); // Raw client without internal Auth handler

            var options = _serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<LHA.BlazorWasm.HttpApi.Client.Options.HttpApiClientOptions>>().Value;
            var baseUri = new Uri(options.BaseAddress);
            var requestUri = new Uri(baseUri, "api/v1/identity/auth/refresh");

            var response = await httpClient.PostAsJsonAsync(requestUri, new RefreshTokenInput { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<LHA.Ddd.Application.ApiResponse<AuthResultDto>>();
                var result = apiResponse?.Result?.Data;

                if (result != null && !string.IsNullOrEmpty(result.AccessToken))
                {
                    _cache.SetToken(result.AccessToken, result.RefreshToken, result.ExpiresIn);
                    await _localStorage.SetAsync(AuthStorageKey, result);
                    return true;
                }
            }
        }
        catch (Exception)
        {
            // Transient faults ignored, fallback to clearing cache
        }

        _cache.Clear();
        await _localStorage.RemoveAsync(AuthStorageKey);

        return false;
    }
}
