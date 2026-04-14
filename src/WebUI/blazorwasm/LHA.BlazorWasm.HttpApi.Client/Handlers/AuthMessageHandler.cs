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
        // Skip adding the token if it's a refresh token request to avoid infinite loops
        if (request.RequestUri?.AbsolutePath.Contains("/identity/auth/refresh", StringComparison.OrdinalIgnoreCase) == true)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        // Reactive Refresh: If 401 Unauthorized, try to refresh and retry
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshedToken = await _tokenProvider.ForceRefreshAsync(token, cancellationToken).ConfigureAwait(false);
            // Only retry if we got a new token that is different from the old one
            if (!string.IsNullOrWhiteSpace(refreshedToken) && refreshedToken != token)
            {
                // Dispose old response and retry with new token
                response.Dispose();
                
                var newRequest = await CloneHttpRequestMessageAsync(request, cancellationToken).ConfigureAwait(false);
                newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
                
                return await base.SendAsync(newRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        return response;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri)
        {
            Version = req.Version
        };

        // Clone content if exists
        if (req.Content != null)
        {
            var ms = new System.IO.MemoryStream();
            await req.Content.CopyToAsync(ms, ct).ConfigureAwait(false);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var h in req.Content.Headers)
            {
                clone.Content.Headers.Add(h.Key, h.Value);
            }
        }

        // Clone headers
        foreach (var h in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
        }

        // Proper header cloning for Authorization will be handled by the caller
        return clone;
    }
}
