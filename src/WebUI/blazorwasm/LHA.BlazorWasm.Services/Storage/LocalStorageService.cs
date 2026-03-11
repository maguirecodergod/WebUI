using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LHA.BlazorWasm.Services.Storage;

/// <summary>
/// Implementation of the Local Storage abstraction service wrapping Blazored.LocalStorage.
/// 
/// Example usage:
/// 
/// // Inject ILocalStorageService (from LHA.BlazorWasm.Services.Storage, not Blazored)
/// await storage.SetAsync("language", "en");
/// var lang = await storage.GetAsync<string>("language");
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly Blazored.LocalStorage.ILocalStorageService _blazoredLocalStorage;
    private readonly StorageOptions _options;

    public LocalStorageService(
        Blazored.LocalStorage.ILocalStorageService blazoredLocalStorage,
        IOptions<StorageOptions> options)
    {
        _blazoredLocalStorage = blazoredLocalStorage ?? throw new ArgumentNullException(nameof(blazoredLocalStorage));
        _options = options?.Value ?? new StorageOptions();
    }

    private string GetFullKey(string key) => $"{_options.KeyPrefix}{key}";

    public async Task SetAsync<T>(string key, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            // Blazored.LocalStorage optimizes JSON serialization internally using System.Text.Json
            await _blazoredLocalStorage.SetItemAsync(fullKey, value);
        }
        catch
        {
            // Prevent exceptions gracefully according to requirements
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);

            // Avoid throwing if key doesn't exist by checking first
            var exists = await _blazoredLocalStorage.ContainKeyAsync(fullKey);
            if (!exists)
            {
                return default;
            }

            return await _blazoredLocalStorage.GetItemAsync<T>(fullKey);
        }
        catch
        {
            // Prevent parsing/missing exceptions
            return default;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            await _blazoredLocalStorage.RemoveItemAsync(fullKey);
        }
        catch
        {
            // Prevent exceptions
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            // Only clear our specific prefix instead of wiping the entire local storage indiscriminately
            var keys = await _blazoredLocalStorage.KeysAsync();
            if (keys != null)
            {
                var prefix = _options.KeyPrefix;
                foreach (var key in keys)
                {
                    if (key.StartsWith(prefix))
                    {
                        await _blazoredLocalStorage.RemoveItemAsync(key);
                    }
                }
            }
        }
        catch
        {
            // Prevent exceptions
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            return await _blazoredLocalStorage.ContainKeyAsync(fullKey);
        }
        catch
        {
            return false;
        }
    }
}
