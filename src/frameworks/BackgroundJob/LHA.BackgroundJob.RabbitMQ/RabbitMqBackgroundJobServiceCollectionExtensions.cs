using LHA.MessageBroker.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.BackgroundJob.RabbitMQ;

/// <summary>
/// Extension methods for registering the RabbitMQ-backed background job system.
/// </summary>
public static class RabbitMqBackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Registers the RabbitMQ <see cref="IBackgroundJobManager"/> and consumer.
    /// Requires that <see cref="RabbitMqConnectionManager"/> is already registered
    /// (e.g. via <c>AddLHARabbitMqMessageBroker</c>).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration for RabbitMQ background job options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHARabbitMqBackgroundJobs(
        this IServiceCollection services,
        Action<RabbitMqBackgroundJobOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLHABackgroundJobs();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Replace the null manager with RabbitMQ
        services.AddSingleton<IBackgroundJobManager, RabbitMqBackgroundJobManager>();

        // Register the consumer as a hosted service
        services.AddHostedService<RabbitMqJobConsumer>();

        return services;
    }
}
