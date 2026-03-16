using Grpc.Core;
using LHA.Grpc.Client.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Grpc.Client;

public static class DependencyInjection
{
    /// <summary>
    /// Registers shared gRPC client interceptors (logging, deadline propagation).
    /// Call once before registering individual gRPC clients.
    /// </summary>
    public static IServiceCollection AddLHAGrpcClientDefaults(
        this IServiceCollection services,
        Action<GrpcClientOptions>? configure = null)
    {
        var options = new GrpcClientOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddSingleton<ClientLoggingInterceptor>();
        services.AddSingleton(new DeadlinePropagationInterceptor(options.DefaultDeadline));
        return services;
    }

    /// <summary>
    /// Registers a typed gRPC client with the LHA interceptor pipeline.
    /// Returns <see cref="IHttpClientBuilder"/> for optional resilience configuration
    /// (e.g. <c>.AddStandardResilienceHandler()</c>).
    /// </summary>
    /// <example>
    /// <code>
    /// services.AddLHAGrpcClient&lt;UserService.UserServiceClient&gt;("https://user-service:5001")
    ///         .AddStandardResilienceHandler();
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddLHAGrpcClient<TClient>(
        this IServiceCollection services,
        string address)
        where TClient : ClientBase<TClient>
    {
        return services
            .AddGrpcClient<TClient>(o => o.Address = new Uri(address))
            .AddInterceptor<ClientLoggingInterceptor>()
            .AddInterceptor<DeadlinePropagationInterceptor>();
    }
}
