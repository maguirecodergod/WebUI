using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using LHA.Auditing.Pipeline;
using LHA.Auditing.Serialization;
using LHA.Core.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// ASP.NET Core middleware that automatically captures audit log records
/// for every HTTP request (Minimal API / Controllers).
/// <para>
/// Must be registered early in the middleware pipeline (before auth)
/// so that the entire request lifecycle is captured.
/// The middleware:
/// 1. Buffers the request body (if configured)
/// 2. Wraps the response body stream (if configured)
/// 3. Measures execution duration
/// 4. Captures user/tenant/client info
/// 5. Submits the record to the pipeline (non-blocking)
/// </para>
/// </summary>
internal sealed class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogCollector collector,
        IOptions<AuditPipelineOptions> options,
        ICurrentUser currentUser,
        IClientInfoProvider clientInfoProvider)
    {
        var opts = options.Value;

        if (!opts.Enabled)
        {
            await _next(context);
            return;
        }

        if (ShouldSkipAuditing(context))
        {
            await _next(context);
            return;
        }

        // Skip excluded paths
        var path = context.Request.Path.Value ?? string.Empty;
        if (opts.ExcludedPaths.Contains(path))
        {
            await _next(context);
            return;
        }

        var record = new AuditLogRecord
        {
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = AuditActionType.HttpRequest,
            RequestType = AuditRequestClassifier.DetectHttpRequestType(context.Request),
            HttpMethod = context.Request.Method,
            RequestPath = $"{context.Request.Path}{context.Request.QueryString}",
            ClientIp = clientInfoProvider.ClientIpAddress,
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            CorrelationId = clientInfoProvider.CorrelationId
        };

        record.Tags = BuildRequestTags(context, record.RequestType);

        // Buffer request body (if configured)
        if (opts.CaptureRequestBody && context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            record.RequestBody = await ReadBodyAsync(context.Request.Body, opts.MaxBodySizeBytes);
            context.Request.Body.Position = 0;
        }

        // Capture response body (if configured)
        Stream? originalResponseBody = null;
        MemoryStream? responseBuffer = null;

        if (opts.CaptureResponseBody)
        {
            originalResponseBody = context.Response.Body;
            responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;
        }

        var sw = Stopwatch.StartNew();
        Exception? caughtException = null;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            caughtException = ex;
            throw;
        }
        finally
        {
            sw.Stop();
            record.DurationMs = sw.ElapsedMilliseconds;

            // Set action name: prefer EndpointName (e.g. "Login"), then DisplayName, then path
            var endpoint = context.GetEndpoint();
            record.ActionName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.EndpointNameMetadata>()?.EndpointName
                               ?? endpoint?.DisplayName
                               ?? path;

            record.StatusCode = context.Response.StatusCode;

            // Capture user info (available after auth middleware)
            record.UserId = currentUser.Id?.ToString();
            record.TenantId = currentUser.TenantId?.ToString();
            record.UserName = currentUser.UserName;
            record.Roles = currentUser.Roles.Length > 0
                ? string.Join(',', currentUser.Roles)
                : null;

            // Capture response body
            if (opts.CaptureResponseBody && responseBuffer is not null && originalResponseBody is not null)
            {
                responseBuffer.Position = 0;
                record.ResponseBody = await ReadBodyAsync(responseBuffer, opts.MaxBodySizeBytes);
                responseBuffer.Position = 0;
                await responseBuffer.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;
                await responseBuffer.DisposeAsync();
            }

            if (caughtException is not null)
            {
                record.Status = AuditLogStatus.Failure;
                record.Exception = AuditExceptionSerializer.Serialize(caughtException);

                // If status code is 200 (default), the outer exception handler hasn't set it yet.
                // We guess it based on the exception type to ensure the log is accurate.
                if (record.StatusCode == 200 || record.StatusCode == 0)
                {
                    record.StatusCode = GuessStatusCode(caughtException);
                }
            }
            else
            {
                record.Status = context.Response.StatusCode >= 400
                    ? AuditLogStatus.Failure
                    : AuditLogStatus.Success;
            }

            // Fire-and-forget to collector (non-blocking)
            collector.Collect(record);
        }
    }

    private static async Task<string?> ReadBodyAsync(Stream stream, int maxSize)
    {
        if (!stream.CanRead)
            return null;

        var buffer = new byte[Math.Min(maxSize, 32 * 1024)];
        var totalRead = 0;
        var sb = new StringBuilder();

        while (totalRead < maxSize)
        {
            var bytesToRead = Math.Min(buffer.Length, maxSize - totalRead);
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));

            if (bytesRead == 0)
                break;

            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            totalRead += bytesRead;
        }

        if (totalRead >= maxSize)
        {
            sb.Append("[truncated]");
        }

        return sb.Length > 0 ? sb.ToString() : null;
    }

    private static int GuessStatusCode(Exception ex) => ex switch
    {
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        ArgumentException or FormatException or InvalidOperationException => StatusCodes.Status400BadRequest,
        OperationCanceledException => 499, // Client Closed Request
        _ when ex.GetType().Name == "EntityNotFoundException" => StatusCodes.Status404NotFound,
        _ when ex.GetType().Name == "BusinessException" => (int?)ex.GetType().GetProperty("StatusCode")?.GetValue(ex) ?? 400,
        _ when ex.GetType().Name == "DbUpdateConcurrencyException" => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string BuildRequestTags(HttpContext context, CRequestType requestType)
    {
        var tags = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["requestType"] = requestType.ToString(),
            ["scheme"] = context.Request.Scheme,
            ["protocol"] = context.Request.Protocol,
            ["host"] = context.Request.Host.Value,
            ["queryString"] = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null
        };

        if (requestType == CRequestType.Webhook)
        {
            tags["webhookEvent"] = context.Request.Headers["X-Webhook-Event"].ToString()
                ?? context.Request.Headers["X-GitHub-Event"].ToString()
                ?? context.Request.Headers["X-Gitlab-Event"].ToString();
        }

        if (requestType == CRequestType.Grpc)
        {
            tags["grpcContentType"] = context.Request.ContentType;
        }

        return System.Text.Json.JsonSerializer.Serialize(tags);
    }


    private static bool ShouldSkipAuditing(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
            return false;

        // 1. Check DisableAuditing trực tiếp trên endpoint
        if (endpoint.Metadata.GetMetadata<DisableAuditingAttribute>() is not null)
            return true;

        // 2. Check controller/action (MVC)
        var actionDescriptor = endpoint.Metadata
            .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

        if (actionDescriptor is not null)
        {
            // Method
            if (actionDescriptor.MethodInfo
                .GetCustomAttributes(typeof(DisableAuditingAttribute), true).Any())
                return true;

            // Class
            if (actionDescriptor.ControllerTypeInfo
                .GetCustomAttributes(typeof(DisableAuditingAttribute), true).Any())
                return true;
        }

        return false;
    }
}
