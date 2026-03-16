using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.UnitOfWork;

/// <summary>
/// Extension methods for registering UnitOfWork services in the DI container.
/// </summary>
public static class UnitOfWorkServiceCollectionExtensions
{
    /// <summary>
    /// Registers the LHA UnitOfWork framework services.
    /// <list type="bullet">
    ///   <item><see cref="AmbientUnitOfWork"/> — singleton (AsyncLocal-based)</item>
    ///   <item><see cref="IUnitOfWorkManager"/> — singleton</item>
    ///   <item><see cref="IUnitOfWork"/> — scoped (one per UoW lifetime)</item>
    ///   <item><see cref="IUnitOfWorkEventPublisher"/> — null-object default (replaced by app)</item>
    /// </list>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional callback to configure <see cref="UnitOfWorkDefaultOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddLHAUnitOfWork(options =>
    /// {
    ///     options.TransactionBehavior = TransactionBehavior.Enabled;
    ///     options.IsolationLevel = IsolationLevel.ReadCommitted;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddLHAUnitOfWork(
        this IServiceCollection services,
        Action<UnitOfWorkDefaultOptions>? configure = null)
    {
        if (configure is not null)
            services.Configure(configure);

        // Ambient tracking (singleton — shared across all scopes via AsyncLocal)
        services.TryAddSingleton<AmbientUnitOfWork>();

        // Manager (singleton — creates new scopes for each UoW)
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();

        // UoW instance (scoped — one per UoW lifetime, created in its own scope)
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();

        // Null-object event publisher (replaced by app-level integration)
        services.TryAddSingleton<IUnitOfWorkEventPublisher, NullUnitOfWorkEventPublisher>();

        return services;
    }
}

/// <summary>
/// Default (no-op) event publisher. Applications replace this by registering
/// their own <see cref="IUnitOfWorkEventPublisher"/> implementation.
/// </summary>
internal sealed class NullUnitOfWorkEventPublisher : IUnitOfWorkEventPublisher
{
    public Task PublishLocalEventsAsync(
        IReadOnlyList<UnitOfWorkEventRecord> localEvents,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task PublishDistributedEventsAsync(
        IReadOnlyList<UnitOfWorkEventRecord> distributedEvents,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
