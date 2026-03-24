using System.Diagnostics;
using LHA.Core.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
            log.ApplicationName = "LHA.WebUI";
            log.UserId = currentUser?.Id;
            log.UserName = currentUser?.UserName;
            log.TenantId = currentUser?.TenantId;
            log.ExecutionTime = DateTimeOffset.UtcNow;
            log.Url = context.Request.Path + context.Request.QueryString;
            log.HttpMethod = context.Request.Method;
            log.ClientIpAddress = context.Connection.RemoteIpAddress?.ToString();
            log.BrowserInfo = context.Request.Headers.UserAgent.ToString();
            log.CorrelationId = Activity.Current?.Id ?? context.TraceIdentifier;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            log?.Exceptions.Add(ex);
            throw;
        }
        finally
        {
            sw.Stop();
            if (log is not null)
            {
                log.HttpStatusCode = context.Response.StatusCode;
                log.ExecutionDuration = (int)sw.ElapsedMilliseconds;
            }
            
            await saveHandle.SaveAsync();
        }
    }
}
