namespace LHA.ApiGateway.Middlewares;

public class TenantResolutionMiddleware(RequestDelegate next)
{
    private const string TenantIdHeaderName = "X-Tenant-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        // Resolve tenant from header
        var tenantId = context.Request.Headers[TenantIdHeaderName].FirstOrDefault();

        // Optionally, resolve from Subdomain
        if (string.IsNullOrEmpty(tenantId))
        {
            var host = context.Request.Host.Host;
            var hostParts = host.Split('.');
            if (hostParts.Length > 2) // e.g. tenant1.example.com
            {
                tenantId = hostParts[0];
            }
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            // Set the extracted tenantId inside the request headers explicitly
            // so YARP forwards it automatically to downstream services
            if (!context.Request.Headers.ContainsKey(TenantIdHeaderName))
            {
                context.Request.Headers.Append(TenantIdHeaderName, tenantId);
            }

            using (Serilog.Context.LogContext.PushProperty("TenantId", tenantId))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
