using LHA.MessageBroker.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.EventBus.RabbitMQ;

/// <summary>
/// Registers the RabbitMQ-backed event bus with outbox/inbox, event versioning, and saga support.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the LHA RabbitMQ event bus implementation.
    /// <para>
    /// Requires <see cref="LHA.MessageBroker.RabbitMQ.DependencyInjection.AddLHARabbitMQ"/> to be called first
    /// to register the underlying RabbitMQ connection/publisher infrastructure.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureEventBus">Configure core event bus options (outbox, inbox, region, etc.).</param>
    /// <param name="configureRabbitMq">Configure RabbitMQ-specific event bus options (exchange mappings, etc.).</param>
    public static IServiceCollection AddLHARabbitMqEventBus(
        this IServiceCollection services,
        Action<EventBusOptions>? configureEventBus = null,
        Action<RabbitMqEventBusOptions>? configureRabbitMq = null)
    {
        // Core abstractions (null stores, options)
        services.AddLHAEventBus(configureEventBus);

        // RabbitMQ-specific options
        if (configureRabbitMq is not null)
            services.Configure(configureRabbitMq);

        // Event bus — RabbitMQ transport
        services.TryAddScoped<IEventBus, RabbitMqEventBus>();

        // Event name resolution
        services.TryAddSingleton<IEventNameResolver, DefaultEventNameResolver>();

        // Saga orchestration
        services.TryAddSingleton<ISagaOrchestrator, SagaOrchestrator>();

        // Event versioning pipeline
        services.TryAddSingleton<EventUpgraderPipeline>();

        // RabbitMQ-aware outbox processor (replaces the generic one)
        services.TryAddScoped<RabbitMqOutboxProcessor>();

        // Inbox processor (shared implementation)
        services.TryAddScoped<InboxProcessor>();

        return services;
    }

    /// <summary>
    /// Starts a background consumer that subscribes to a RabbitMQ exchange/queue
    /// and dispatches events through the EventBus handler pipeline with inbox deduplication.
    /// </summary>
    public static IServiceCollection AddRabbitMqEventConsumer(this IServiceCollection services)
    {
        services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(sp =>
            new RabbitMqEventConsumerBackgroundService(
                sp.GetRequiredService<RabbitMqConnectionManager>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<IEventNameResolver>(),
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EventBusOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqEventBusOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RabbitMqEventConsumerBackgroundService>>()));

        return services;
    }
}
