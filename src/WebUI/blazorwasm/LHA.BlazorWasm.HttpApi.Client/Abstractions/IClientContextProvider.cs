namespace LHA.BlazorWasm.HttpApi.Client.Abstractions;

/// <summary>
/// Provides client context information such as Tenant, Device, and Version for API requests.
/// </summary>
public interface IClientContextProvider
{
    string? GetTenantId();
    string? GetDeviceId();
    string? GetClientVersion();
    string? GetApiKey();
    string? GetAcceptLanguage();
}
