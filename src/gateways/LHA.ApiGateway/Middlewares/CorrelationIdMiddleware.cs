namespace LHA.ApiGateway.Middlewares;

using System.Diagnostics;
using Microsoft.Extensions.Primitives;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrGenerateCorrelationId(context);

        // Add to Request header (to pass to downstream services via YARP)
        if (!context.Request.Headers.ContainsKey(CorrelationIdHeaderName))
        {
            context.Request.Headers.TryAdd(CorrelationIdHeaderName, correlationId);
        }

        // Add to Response header
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(CorrelationIdHeaderName))
            {
                context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);
            }
            return Task.CompletedTask;
        });

        // Add to Activity/Tracing scope
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetBaggage(CorrelationIdHeaderName, correlationId);
            activity.AddTag(CorrelationIdHeaderName, correlationId);
        }

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues headerValues))
        {
            var id = headerValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }
        }
        return Guid.NewGuid().ToString("N");
    }
}
