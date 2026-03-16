using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.EventBus;

/// <summary>
/// Registers the in-memory event bus implementation and supporting services.
/// </summary>
public static class InMemoryEventBusServiceCollectionExtensions
{
    /// <summary>
    /// Adds the LHA in-memory event bus with saga orchestration, event versioning,
    /// and optional outbox/inbox processing.
    /// <para>
    /// Call <see cref="EventBusServiceCollectionExtensions.AddLHAEventBus"/> implicitly
    /// through this method, which registers all core + in-memory services.
    /// </para>
    /// </summary>
    public static IServiceCollection AddLHAInMemoryEventBus(
        this IServiceCollection services,
        Action<EventBusOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Register core abstractions + null stores
        services.AddLHAEventBus(configure);

        // In-memory event bus (Scoped — depends on IOutboxStore which may be scoped when backed by EF Core)
        services.TryAddScoped<IEventBus, InMemoryEventBus>();

        // Event name resolution
        services.TryAddSingleton<IEventNameResolver, DefaultEventNameResolver>();

        // Saga orchestration
        services.TryAddSingleton<ISagaOrchestrator, SagaOrchestrator>();

        // Event versioning pipeline
        services.TryAddSingleton<EventUpgraderPipeline>();

        // Outbox/Inbox processors (Scoped — depend on stores that may be scoped when backed by EF Core)
        services.TryAddScoped<OutboxProcessor>();
        services.TryAddScoped<InboxProcessor>();

        return services;
    }
}
