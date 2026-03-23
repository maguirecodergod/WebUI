using System.Security.Claims;
using System.Threading.RateLimiting;
using LHA.ApiGateway.ReverseProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

namespace LHA.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all gateway infrastructure, YARP, auth, and rate limiters.
    /// </summary>
    public static IServiceCollection AddGatewayInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Dependency Injection
        services.AddSingleton<ProxyConfigProvider>();

        // 2. YARP Reverse Proxy
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddTransforms(builderContext =>
            {
                // Optionally inject custom dynamic transforms
                builderContext.AddRequestTransform(transformContext =>
                {
                    // Propagate User Id if authenticated
                    if (transformContext.HttpContext.User.Identity?.IsAuthenticated == true)
                    {
                        var userId = transformContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
                        }
                    }
                    return ValueTask.CompletedTask;
                });
            });

        // 3. Authentication & JWT Integration
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Authentication:Authority"];
                options.Audience = configuration["Authentication:Audience"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("Authentication:RequireHttpsMetadata");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization();

        // 4. Rate Limiting
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            var limit = configuration.GetValue<int>("RateLimiting:GlobalRateLimit", 100);
            var window = configuration.GetValue<int>("RateLimiting:GlobalWindowSeconds", 60);

            options.AddPolicy("GlobalLimit", context =>
            {
                // Rate limit by IP or User ID if authenticated
                var partitionKey = context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!
                    : context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = limit,
                    Window = TimeSpan.FromSeconds(window),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
        });

        // 5. CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}
