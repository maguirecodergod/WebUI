using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.EventBus;

/// <summary>
/// Registers core event bus abstractions and options.
/// Transport-specific implementations (InMemory, Kafka, RabbitMQ) add their own extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds core LHA event bus services: options configuration and null stores.
    /// Call a transport-specific extension (e.g., <c>AddLHAInMemoryEventBus</c>) to register
    /// <see cref="IEventBus"/> and background processors.
    /// </summary>
    public static IServiceCollection AddLHAEventBus(
        this IServiceCollection services,
        Action<EventBusOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Null stores as defaults — replaced by EF Core / Dapper / Redis implementations
        services.TryAddSingleton<IOutboxStore, NullOutboxStore>();
        services.TryAddSingleton<IInboxStore, NullInboxStore>();

        return services;
    }
}
