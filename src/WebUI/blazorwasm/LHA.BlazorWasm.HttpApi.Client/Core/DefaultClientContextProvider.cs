using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Options;
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

    /// <summary>
    /// Initializes a new instance of the DefaultClientContextProvider class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="localizationState"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultClientContextProvider(
        IOptions<HttpApiClientOptions> options, 
        LocalizationState localizationState)
    {
        _options = options.Value;
        _localizationState = localizationState ?? throw new ArgumentNullException(nameof(localizationState));
    }
    
    /// <summary>
    /// Gets the tenant ID from the client context.
    /// </summary>
    /// <returns></returns>
    public virtual string? GetTenantId() => null; // Typically resolved from auth state or host
    /// <summary>
    /// Gets the device ID from the client context.
    /// </summary>
    /// <returns></returns>
    public virtual string? GetDeviceId() => null; // Typically resolved from local storage
    /// <summary>
    /// Gets the client version from the client context.
    /// </summary>
    /// <returns></returns>
    public virtual string? GetClientVersion() => "1.0.0"; 
    /// <summary>
    /// Gets the API key from the client context.
    /// </summary>
    /// <returns></returns>
    public virtual string? GetApiKey() => null;
    /// <summary>
    /// Gets the accept language from the client context.
    /// </summary>
    /// <returns></returns>
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
