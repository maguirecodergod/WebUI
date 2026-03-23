using LHA.ApiGateway.Middlewares;

namespace LHA.ApiGateway.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the HTTP request pipeline with Custom Middlewares, Auth, and YARP.
    /// </summary>
    public static WebApplication UseGatewayPipeline(this WebApplication app)
    {
        // 1. Initial protections & fundamentals
        app.UseHttpsRedirection();
        app.UseCors();
        
        // 2. Custom Observability & Multi-Tenancy middlewares
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<TenantResolutionMiddleware>();

        // 3. Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // 4. Resilience & Rate Limiting
        app.UseRateLimiter();

        // 5. Map Reverse Proxy
        app.MapReverseProxy();

        return app;
    }
}
