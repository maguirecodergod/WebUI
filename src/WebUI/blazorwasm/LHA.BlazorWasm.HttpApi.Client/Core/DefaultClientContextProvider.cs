using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Options;
using LHA.BlazorWasm.Shared.Abstractions.Localization;
using LHA.BlazorWasm.Shared.Models.Localization;
using Microsoft.Extensions.Options;

namespace LHA.BlazorWasm.HttpApi.Client.Core;

/// <summary>
/// A default implementation of IClientContextProvider that retrieves static values from configuration.
/// </summary>
public class DefaultClientContextProvider : IClientContextProvider
{
    private readonly HttpApiClientOptions _options;
    private readonly LocalizationState _localizationState;

    public DefaultClientContextProvider(
        IOptions<HttpApiClientOptions> options, 
        LocalizationState localizationState)
    {
        _options = options.Value;
        _localizationState = localizationState ?? throw new ArgumentNullException(nameof(localizationState));
    }

    public virtual string? GetTenantId() => null; // Typically resolved from auth state or host
    public virtual string? GetDeviceId() => null; // Typically resolved from local storage
    public virtual string? GetClientVersion() => "1.0.0"; 
    public virtual string? GetApiKey() => null;
    
    public virtual string? GetAcceptLanguage() 
    {
        var culture = _localizationState?.CurrentCulture;
        
        if (!string.IsNullOrWhiteSpace(culture))
        {
            return culture;
        }

        return System.Globalization.CultureInfo.CurrentCulture.Name;
    }
}
