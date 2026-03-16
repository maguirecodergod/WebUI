using LHA.Grpc.Server.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Grpc.Server;

public static class DependencyInjection
{
    /// <summary>
    /// Registers gRPC server infrastructure with the LHA interceptor pipeline:
    /// Metrics → Logging → Exception → (RateLimit).
    /// </summary>
    public static IServiceCollection AddLHAGrpcServer(
        this IServiceCollection services,
        Action<GrpcServerOptions>? configure = null)
    {
        var options = new GrpcServerOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        if (options.RateLimiter is not null)
            services.AddSingleton(options.RateLimiter);

        services.AddGrpc(grpc =>
        {
            grpc.EnableDetailedErrors = options.EnableDetailedErrors;

            if (options.MaxReceiveMessageSize.HasValue)
                grpc.MaxReceiveMessageSize = options.MaxReceiveMessageSize.Value;
            if (options.MaxSendMessageSize.HasValue)
                grpc.MaxSendMessageSize = options.MaxSendMessageSize.Value;
            if (options.ResponseCompressionAlgorithm is not null)
            {
                grpc.ResponseCompressionAlgorithm = options.ResponseCompressionAlgorithm;
                grpc.ResponseCompressionLevel = options.ResponseCompressionLevel;
            }

            // Interceptor pipeline (outermost → innermost):
            grpc.Interceptors.Add<MetricsInterceptor>();
            grpc.Interceptors.Add<LoggingInterceptor>();
            grpc.Interceptors.Add<ExceptionInterceptor>();

            if (options.RateLimiter is not null)
                grpc.Interceptors.Add<RateLimitInterceptor>();
        });

        if (options.EnableReflection)
            services.AddGrpcReflection();

        return services;
    }

    /// <summary>
    /// Maps gRPC infrastructure endpoints (reflection service, health checks).
    /// Call after mapping your gRPC services.
    /// </summary>
    public static IEndpointRouteBuilder MapLHAGrpcInfrastructure(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider.GetRequiredService<GrpcServerOptions>();

        if (options.EnableReflection)
            endpoints.MapGrpcReflectionService();

        return endpoints;
    }
}
