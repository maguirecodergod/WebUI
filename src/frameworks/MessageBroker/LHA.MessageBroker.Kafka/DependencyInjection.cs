using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Extension methods for registering the LHA Kafka MessageBroker
/// in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds LHA Kafka MessageBroker services to the service collection.
    /// Registers <see cref="IMessagePublisher"/> backed by Kafka.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configure Kafka options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHAKafka(
        this IServiceCollection services,
        Action<KafkaOptions> configure)
    {
        services.Configure(configure);

        // Core infrastructure
        services.TryAddSingleton<KafkaConnectionFactory>();
        services.TryAddSingleton<IMessageSerializer, SystemTextJsonMessageSerializer>();

        // Producer — registered as both IMessagePublisher (broker-agnostic) and KafkaProducer (specific)
        services.TryAddSingleton<KafkaProducer>();
        services.TryAddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<KafkaProducer>());

        // Health check
        services.TryAddSingleton<IMessageBrokerHealthCheck, KafkaHealthCheck>();

        return services;
    }

    /// <summary>
    /// Registers a message handler and starts a background consumer for the specified topic.
    /// </summary>
    /// <typeparam name="TPayload">The message payload type.</typeparam>
    /// <typeparam name="THandler">The message handler implementation.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="topic">The Kafka topic to consume.</param>
    /// <param name="tenantIdFilter">Optional: only process messages for this tenant.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddKafkaConsumer<TPayload, THandler>(
        this IServiceCollection services,
        string topic,
        string? tenantIdFilter = null)
        where TPayload : class
        where THandler : class, IMessageHandler<TPayload>
    {
        // Register handler
        services.TryAddScoped<IMessageHandler<TPayload>, THandler>();

        // Register background service
        services.AddSingleton<IHostedService>(sp =>
        {
            return new KafkaConsumerBackgroundService<TPayload>(
                sp.GetRequiredService<KafkaConnectionFactory>(),
                sp.GetRequiredService<IMessageSerializer>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<IOptions<KafkaOptions>>(),
                sp.GetRequiredService<ILogger<KafkaConsumerBackgroundService<TPayload>>>(),
                topic,
                tenantIdFilter);
        });

        return services;
    }

    /// <summary>
    /// Configures a specific topic with custom options.
    /// </summary>
    public static IServiceCollection ConfigureKafkaTopic(
        this IServiceCollection services,
        string topic,
        Action<KafkaTopicOptions> configure)
    {
        services.PostConfigure<KafkaOptions>(options =>
        {
            var topicOptions = options.GetTopicOptions(topic);
            configure(topicOptions);
        });

        return services;
    }
}
