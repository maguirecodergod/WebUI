using System.Net.Http.Headers;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;

namespace LHA.BlazorWasm.HttpApi.Client.Handlers;

/// <summary>
/// Attaches the Authorization header to outgoing requests.
/// </summary>
public class AuthMessageHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;

    public AuthMessageHandler(IAccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
