using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.MessageBroker.RabbitMQ;

/// <summary>
/// Extension methods for registering the LHA RabbitMQ MessageBroker
/// in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds LHA RabbitMQ MessageBroker services to the service collection.
    /// Registers <see cref="IMessagePublisher"/> backed by RabbitMQ.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configure RabbitMQ options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHARabbitMQ(
        this IServiceCollection services,
        Action<RabbitMqOptions> configure)
    {
        services.Configure(configure);

        // Core infrastructure
        services.TryAddSingleton<RabbitMqConnectionManager>();
        services.TryAddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();

        // Publisher — registered as both IMessagePublisher (broker-agnostic) and specific type
        services.TryAddSingleton<RabbitMqMessagePublisher>();
        services.TryAddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<RabbitMqMessagePublisher>());

        // Health check
        services.TryAddSingleton<IMessageBrokerHealthCheck, RabbitMqHealthCheck>();

        return services;
    }

    /// <summary>
    /// Registers a message handler and starts a background consumer for the specified exchange/queue.
    /// </summary>
    /// <typeparam name="TPayload">The message payload type.</typeparam>
    /// <typeparam name="THandler">The message handler implementation.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configure the subscription (exchange, queue, routing key, etc.).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddRabbitMqConsumer<TPayload, THandler>(
        this IServiceCollection services,
        Action<RabbitMqSubscriptionOptions> configure)
        where TPayload : class
        where THandler : class, IMessageHandler<TPayload>
    {
        // Build subscription options
        var subscription = new RabbitMqSubscriptionOptions
        {
            Exchange = typeof(TPayload).Name.ToLowerInvariant(),
            Queue = $"{typeof(TPayload).Name.ToLowerInvariant()}.queue"
        };
        configure(subscription);

        // Register handler
        services.TryAddScoped<IMessageHandler<TPayload>, THandler>();

        // Register background service
        services.AddSingleton<IHostedService>(sp =>
        {
            return new RabbitMqConsumerBackgroundService<TPayload>(
                sp.GetRequiredService<RabbitMqConnectionManager>(),
                sp.GetRequiredService<IMessageSerializer>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<IOptions<RabbitMqOptions>>(),
                subscription,
                sp.GetRequiredService<ILogger<RabbitMqConsumerBackgroundService<TPayload>>>());
        });

        return services;
    }

    /// <summary>
    /// Simplified overload: registers a consumer with explicit exchange and queue names.
    /// </summary>
    public static IServiceCollection AddRabbitMqConsumer<TPayload, THandler>(
        this IServiceCollection services,
        string exchange,
        string queue,
        string routingKey = "#",
        string? tenantIdFilter = null)
        where TPayload : class
        where THandler : class, IMessageHandler<TPayload>
    {
        return services.AddRabbitMqConsumer<TPayload, THandler>(opts =>
        {
            opts.Exchange = exchange;
            opts.Queue = queue;
            opts.RoutingKey = routingKey;
            opts.TenantIdFilter = tenantIdFilter;
        });
    }
}
