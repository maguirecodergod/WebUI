using LHA.MessageBroker.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.EventBus.Kafka;

/// <summary>
/// Registers the Kafka-backed event bus with outbox/inbox, event versioning, and saga support.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the LHA Kafka event bus implementation.
    /// <para>
    /// Requires <see cref="LHA.MessageBroker.Kafka.DependencyInjection.AddLHAKafka"/> to be called first
    /// to register the underlying Kafka producer/consumer infrastructure.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureEventBus">Configure core event bus options (outbox, inbox, region, etc.).</param>
    /// <param name="configureKafka">Configure Kafka-specific event bus options (topic mappings, etc.).</param>
    public static IServiceCollection AddLHAKafkaEventBus(
        this IServiceCollection services,
        Action<EventBusOptions>? configureEventBus = null,
        Action<KafkaEventBusOptions>? configureKafka = null)
    {
        // Core abstractions (null stores, options)
        services.AddLHAEventBus(configureEventBus);

        // Kafka-specific options
        if (configureKafka is not null)
            services.Configure(configureKafka);

        // Event bus — Kafka transport
        services.TryAddScoped<IEventBus, KafkaEventBus>();

        // Event name resolution
        services.TryAddSingleton<IEventNameResolver, DefaultEventNameResolver>();

        // Saga orchestration
        services.TryAddSingleton<ISagaOrchestrator, SagaOrchestrator>();

        // Event versioning pipeline
        services.TryAddSingleton<EventUpgraderPipeline>();

        // Kafka-aware outbox processor (replaces the generic one)
        services.TryAddScoped<KafkaOutboxProcessor>();

        // Inbox processor (shared implementation)
        services.TryAddScoped<InboxProcessor>();

        return services;
    }

    /// <summary>
    /// Registers a background consumer that subscribes to a Kafka topic and dispatches
    /// events through the EventBus handler pipeline with inbox deduplication.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="topic">The Kafka topic to consume events from.</param>
    public static IServiceCollection AddKafkaEventConsumer(
        this IServiceCollection services,
        string topic)
    {
        services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService>(sp =>
            new KafkaEventConsumerBackgroundService(
                sp.GetRequiredService<KafkaConnectionFactory>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<IEventNameResolver>(),
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EventBusOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<KafkaEventBusOptions>>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<KafkaEventConsumerBackgroundService>>(),
                topic));

        return services;
    }

    /// <summary>
    /// Registers a background service that periodically reads pending outbox messages
    /// and forwards them to Kafka. Typically called in the producer/host project.
    /// </summary>
    public static IServiceCollection AddKafkaOutboxProcessor(
        this IServiceCollection services)
    {
        services.AddHostedService<KafkaOutboxProcessorBackgroundService>();
        return services;
    }
}
