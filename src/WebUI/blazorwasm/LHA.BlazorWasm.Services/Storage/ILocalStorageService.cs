using System.Threading.Tasks;

namespace LHA.BlazorWasm.Services.Storage;

/// <summary>
/// Interface for a Local Storage abstraction service.
/// </summary>
public interface ILocalStorageService
{
    /// <summary>
    /// Sets an item in local storage as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T">The type of the object to set.</typeparam>
    /// <param name="key">The key identifying the item.</param>
    /// <param name="value">The object to store.</param>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Gets an item from local storage as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T">The type of the expected object.</typeparam>
    /// <param name="key">The key identifying the item.</param>
    /// <returns>The deserialized object or default if not found/error.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Removes an item from local storage as an asynchronous operation.
    /// </summary>
    /// <param name="key">The key identifying the item to remove.</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Clears all stored items (with the configured prefix) as an asynchronous operation.
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Checks if a key exists in local storage as an asynchronous operation.
    /// </summary>
    /// <param name="key">The key identifying the item.</param>
    /// <returns>True if the item exists, otherwise false.</returns>
    Task<bool> ExistsAsync(string key);
}
