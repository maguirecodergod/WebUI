using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.Shared.Constants;

namespace LHA.BlazorWasm.HttpApi.Client.Handlers;

/// <summary>
/// Injects standard enterprise context variables (Tenant, Device, API Key, Locale) into HTTP request headers.
/// </summary>
public class ContextMessageHandler : DelegatingHandler
{
    private readonly IClientContextProvider _contextProvider;

    /// <summary>
    /// Initializes a new instance of the ContextMessageHandler class.
    /// </summary>
    /// <param name="contextProvider"></param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public ContextMessageHandler(IClientContextProvider contextProvider)
    {
        _contextProvider = contextProvider ?? throw new System.ArgumentNullException(nameof(contextProvider));
    }

    /// <summary>
    /// Sends the HTTP request to the inner handler in the pipeline.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        TryAddHeader(request, CustomHttpHeaderNames.TenantId, _contextProvider.GetTenantId());
        TryAddHeader(request, CustomHttpHeaderNames.DeviceId, _contextProvider.GetDeviceId());
        TryAddHeader(request, CustomHttpHeaderNames.ClientVersion, _contextProvider.GetClientVersion());
        TryAddHeader(request, CustomHttpHeaderNames.ApiKey, _contextProvider.GetApiKey());

        var locale = _contextProvider.GetAcceptLanguage();
        if (!string.IsNullOrWhiteSpace(locale))
        {
            try
            {
                request.Headers.AcceptLanguage.Clear();
                request.Headers.AcceptLanguage.ParseAdd(locale);
                request.Headers.TryAddWithoutValidation("X-App-Locale", locale);
            }
            catch { /* Ignore invalid language formats */ }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static void TryAddHeader(HttpRequestMessage request, string headerName, string? headerValue)
    {
        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            request.Headers.TryAddWithoutValidation(headerName, headerValue);
        }
    }
}
