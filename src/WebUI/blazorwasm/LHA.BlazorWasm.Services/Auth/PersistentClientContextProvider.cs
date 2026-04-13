using LHA.BlazorWasm.HttpApi.Client.Abstractions;

namespace LHA.BlazorWasm.Services.Auth;

/// <summary>
/// Implementation of IClientContextProvider that provides the Tenant ID from an in-memory cache.
/// The cache is updated by AuthStateProvider and persisted in LocalStorage.
/// </summary>
public class PersistentClientContextProvider : IClientContextProvider
{
    private string? _tenantId;

    public string? GetTenantId() => _tenantId;

    public void SetTenantId(string? tenantId)
    {
        _tenantId = tenantId;
    }

    public string? GetDeviceId() => null;
    public string? GetClientVersion() => "1.0.0";
    public string? GetApiKey() => null;

    public string? GetAcceptLanguage() => System.Globalization.CultureInfo.CurrentCulture.Name;
}
