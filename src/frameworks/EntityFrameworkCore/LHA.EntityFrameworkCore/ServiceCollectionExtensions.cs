using LHA.Ddd.Domain;
using LHA.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> for registering
/// EF Core DbContexts and repositories with the LHA framework.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <typeparamref name="TDbContext"/> with the DI container,
    /// wired into the LHA Unit of Work and DbContext options pipeline.
    /// </summary>
    /// <typeparam name="TDbContext">The DbContext type to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">
    /// Optional callback to configure <see cref="LhaDbContextOptions"/> for this context type.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddLhaDbContext<TDbContext>(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
        where TDbContext : DbContext
    {
        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }

        services.AddDbContext<TDbContext>((sp, builder) =>
        {
            var options = sp.GetRequiredService<IOptions<LhaDbContextOptions>>().Value;

            // Prefer per-type config, fall back to default.
            if (options.ConfigureActions.TryGetValue(typeof(TDbContext), out var configureAction))
            {
                configureAction(CreateConfigurationContext(builder));
            }
            else
            {
                options.DefaultConfigureAction?.Invoke(CreateConfigurationContext(builder));
            }
        });

        services.TryAddScoped<IDbContextProvider<TDbContext>, UnitOfWorkDbContextProvider<TDbContext>>();

        // Auto-register outbox/inbox stores when the DbContext opts in.
        TryAddEventStores<TDbContext>(services);

        return services;
    }

    /// <summary>
    /// Registers an <see cref="EfCoreRepository{TDbContext,TEntity,TKey}"/> for the specified
    /// entity type as both <see cref="IRepository{TEntity,TKey}"/> and <see cref="IEfCoreRepository{TEntity,TKey}"/>.
    /// </summary>
    public static IServiceCollection AddEfCoreRepository<TDbContext, TEntity, TKey>(
        this IServiceCollection services)
        where TDbContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TKey : notnull
    {
        services.TryAddScoped<IRepository<TEntity, TKey>, EfCoreRepository<TDbContext, TEntity, TKey>>();
        services.TryAddScoped<IReadOnlyRepository<TEntity, TKey>, EfCoreRepository<TDbContext, TEntity, TKey>>();
        services.TryAddScoped<IEfCoreRepository<TEntity, TKey>, EfCoreRepository<TDbContext, TEntity, TKey>>();
        return services;
    }

    private static LhaDbContextConfigurationContext CreateConfigurationContext(
        DbContextOptionsBuilder builder,
        string? connectionString = null)
    {
        return new LhaDbContextConfigurationContext
        {
            ConnectionString = connectionString ?? string.Empty,
            DbContextOptions = builder
        };
    }

    /// <summary>
    /// Checks if <typeparamref name="TDbContext"/> implements <see cref="IHasEventOutbox"/>
    /// and/or <see cref="IHasEventInbox"/> and registers the matching EF Core store implementations.
    /// This replaces the <see cref="NullOutboxStore"/>/<see cref="NullInboxStore"/> defaults.
    /// </summary>
    private static void TryAddEventStores<TDbContext>(IServiceCollection services)
        where TDbContext : DbContext
    {
        var dbContextType = typeof(TDbContext);

        if (typeof(IHasEventOutbox).IsAssignableFrom(dbContextType))
        {
            var storeType = typeof(EfCoreOutboxStore<>).MakeGenericType(dbContextType);
            services.Replace(ServiceDescriptor.Scoped(typeof(IOutboxStore), storeType));
        }

        if (typeof(IHasEventInbox).IsAssignableFrom(dbContextType))
        {
            var storeType = typeof(EfCoreInboxStore<>).MakeGenericType(dbContextType);
            services.Replace(ServiceDescriptor.Scoped(typeof(IInboxStore), storeType));
        }
    }
}
