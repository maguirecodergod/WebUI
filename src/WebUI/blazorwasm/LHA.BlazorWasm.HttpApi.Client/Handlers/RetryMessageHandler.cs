using System.Net;
using Microsoft.Extensions.Logging;

namespace LHA.BlazorWasm.HttpApi.Client.Handlers;

/// <summary>
/// Extends HTTP requests with retry capability to handle transient faults (5xx, timeouts, network issues).
/// </summary>
public class RetryMessageHandler : DelegatingHandler
{
    private readonly ILogger<RetryMessageHandler> _logger;
    private readonly int _maxRetries;

    public RetryMessageHandler(ILogger<RetryMessageHandler> logger, int maxRetries = 3)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _maxRetries = maxRetries;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int retryCount = 0;
        
        while (true)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (IsTransientError(response.StatusCode) && retryCount < _maxRetries)
                {
                    _logger.LogWarning("Transient HTTP error {StatusCode}. Retrying ({RetryCount}/{MaxRetries}) for {Url}...", 
                        response.StatusCode, retryCount + 1, _maxRetries, request.RequestUri);
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex) when (IsNetworkOrTimeoutException(ex) && retryCount < _maxRetries)
            {
                _logger.LogWarning(ex, "Network or timeout error. Retrying ({RetryCount}/{MaxRetries}) for {Url}...", 
                    retryCount + 1, _maxRetries, request.RequestUri);
            }

            retryCount++;
            var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount)); // Exponential backoff: 2, 4, 8 secs
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    private static bool IsTransientError(HttpStatusCode statusCode)
    {
        return (int)statusCode >= 500 || statusCode == HttpStatusCode.RequestTimeout;
    }

    private static bool IsNetworkOrTimeoutException(Exception ex)
    {
        return ex is HttpRequestException || ex is TaskCanceledException || ex is TimeoutException;
    }
}
