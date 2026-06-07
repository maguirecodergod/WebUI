namespace LHA.BlazorWasm.HttpApi.Client.Abstractions;

/// <summary>
/// Provides client context information such as Tenant, Device, and Version for API requests.
/// </summary>
public interface IClientContextProvider
{
    /// <summary>
    /// Gets the tenant ID from the client context.
    /// </summary>
    /// <returns></returns>
    string? GetTenantId();
    /// <summary>
    /// Gets the device ID from the client context.
    /// </summary>
    /// <returns></returns>
    string? GetDeviceId();
    /// <summary>
    /// Gets the client version from the client context.
    /// </summary>
    /// <returns></returns>
    string? GetClientVersion();
    /// <summary>
    /// Gets the API key from the client context.
    /// </summary>
    /// <returns></returns>
    string? GetApiKey();
    /// <summary>
    /// Gets the accept language from the client context.
    /// </summary>
    /// <returns></returns>
    string? GetAcceptLanguage();
}
