using System.Diagnostics;
using LHA.Core.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;

namespace LHA.Auditing.Interceptors;

/// <summary>
/// Middleware for the classic Data Auditing system.
/// Begins an IAuditingManager scope, captures request context, and executes.
/// </summary>
internal sealed class DataAuditingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var auditingManager = context.RequestServices.GetService<IAuditingManager>();
        if (auditingManager is null)
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        
        await using var saveHandle = auditingManager.BeginScope();
        
        var log = auditingManager.Current?.Log;
        if (log is not null)
        {
            var currentUser = context.RequestServices.GetService<ICurrentUser>();
            // log.ApplicationName is set by AuditingManager from options
            log.UserId = currentUser?.Id;
            log.UserName = currentUser?.UserName;
            log.TenantId = currentUser?.TenantId;
            log.ExecutionTime = DateTimeOffset.UtcNow;
            log.Url = context.Request.Path + context.Request.QueryString;
            log.HttpMethod = context.Request.Method;
            log.ClientIpAddress = context.Connection.RemoteIpAddress?.ToString();
            log.BrowserInfo = context.Request.Headers.UserAgent.ToString();
            log.CorrelationId = Activity.Current?.Id ?? context.TraceIdentifier;

            // Resolve ActionName: prefer EndpointName, then DisplayName, then path
            var endpoint = context.GetEndpoint();
            log.ActionName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.EndpointNameMetadata>()?.EndpointName
                             ?? endpoint?.DisplayName
                             ?? context.Request.Path;

            // Capture request body if enabled
            var options = context.RequestServices.GetRequiredService<IOptions<AuditingOptions>>().Value;
            if (options.CaptureRequestBody && context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                var bodyString = await ReadBodyAsync(context.Request.Body);
                context.Request.Body.Position = 0;

                // Store in ExtraProperties or we could add it to the first Action later
                log.ExtraProperties["RequestBody"] = bodyString;
            }
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (log is not null)
            {
                log.Exceptions.Add(ex);
                // Guess status code if not set (outer handler hasn't run yet)
                if (log.HttpStatusCode is null or 200)
                {
                    log.HttpStatusCode = GuessStatusCode(ex);
                }
            }
            throw;
        }
        finally
        {
            sw.Stop();
            if (log is not null)
            {
                // Only overwrite if it was 200/null (indicating no exception guessed it yet)
                // or if the response actually has a real status code set by another middleware
                if (log.HttpStatusCode is null or 200 && context.Response.StatusCode != 200)
                {
                    log.HttpStatusCode = context.Response.StatusCode;
                }
                
                log.ExecutionDuration = (int)sw.ElapsedMilliseconds;

                // Add at least one action to the log to satisfy AuditLogActionEntity requirements
                if (log.Actions.Count == 0)
                {
                    log.Actions.Add(new AuditLogAction
                    {
                        ServiceName = log.ApplicationName ?? "Application",
                        MethodName = log.ActionName ?? context.Request.Path,
                        ExecutionTime = log.ExecutionTime,
                        ExecutionDuration = log.ExecutionDuration,
                        Parameters = log.ExtraProperties.TryGetValue("RequestBody", out var body) ? body?.ToString() : null
                    });
                }
            }
            
            await saveHandle.SaveAsync();
        }
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

    private static async Task<string?> ReadBodyAsync(Stream bodyStream)
    {
        using var reader = new StreamReader(bodyStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        return body;
    }
}
