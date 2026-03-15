using Microsoft.Extensions.Logging;
using System.Diagnostics;
using LHA.BlazorWasm.Shared.Constants;

namespace LHA.BlazorWasm.HttpApi.Client.Handlers;

/// <summary>
/// Logs all outgoing HTTP requests and responses, injecting correlation IDs.
/// </summary>
public class LoggingMessageHandler : DelegatingHandler
{
    private readonly ILogger<LoggingMessageHandler> _logger;

    public LoggingMessageHandler(ILogger<LoggingMessageHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        request.Headers.TryAddWithoutValidation(CustomHttpHeaderNames.CorrelationId, correlationId);
        request.Headers.TryAddWithoutValidation(CustomHttpHeaderNames.RequestId, correlationId);

        _logger.LogInformation("HTTP {Method} {Url} [CorrelationId: {CorrelationId}]", request.Method, request.RequestUri, correlationId);

        var start = Stopwatch.GetTimestamp();
        
        try
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            _logger.LogInformation("HTTP {Method} {Url} responded {StatusCode} in {ElapsedMs:0.0000}ms [CorrelationId: {CorrelationId}]", 
                request.Method, request.RequestUri, (int)response.StatusCode, elapsedMs, correlationId);
                
            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("HTTP {Method} {Url} was cancelled. [CorrelationId: {CorrelationId}]", request.Method, request.RequestUri, correlationId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP {Method} {Url} failed. [CorrelationId: {CorrelationId}]", request.Method, request.RequestUri, correlationId);
            throw;
        }
    }
}
