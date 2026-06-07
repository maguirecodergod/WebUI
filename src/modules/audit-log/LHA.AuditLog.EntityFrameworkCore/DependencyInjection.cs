#pragma warning disable
#pragma warning restore

using LHA.Auditing;
using LHA.Auditing.Pipeline;
using LHA.AuditLog.Domain.Shared;
using LHA.AuditLog.EntityFrameworkCore.Contracts.Options;
using LHA.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Registers EntityFrameworkCore infrastructure for the Audit Log module.
/// Provider-specific implementations (PostgreSQL, MongoDB) must be registered separately.
/// </summary>
public static class AuditLogEntityFrameworkCoreDependencyInjection
{
    /// <summary>
    /// Registers the AuditLogDbContext, audit stores, and pipeline dispatcher.
    /// Provider-specific repositories (PostgreSQL/MongoDB) are registered separately.
    /// </summary>
    [Obsolete("Use Register instead. Because AddAuditLogEntityFrameworkCore is not thread-safe. ")]
    public static IServiceCollection AddAuditLogEntityFrameworkCore(
        this IServiceCollection services,
        Action<AuditLogEntityFrameworkCoreBuilder>? configure = null)
    {
        var builder = new AuditLogEntityFrameworkCoreBuilder();
        configure?.Invoke(builder);

        return Register(services, builder);
    }


    public static IServiceCollection Register(
        IServiceCollection services,
        AuditLogEntityFrameworkCoreBuilder builder)
    {
        RegisterOptions(services, builder);

        services.AddLhaDbContext<AuditLogDbContext>(builder.DbContextConfig);

        if (builder.Mode.HasFlag(CAuditLogStoreMode.DataAudit))
        {
            services.Replace(
                ServiceDescriptor.Scoped<IAuditingStore, EfCoreAuditingStore>());
        }

        if (builder.Mode.HasFlag(CAuditLogStoreMode.Pipeline))
        {
            services.AddSingleton<IAuditLogDispatcher, EfCoreAuditLogDispatcher>();
        }

        return services;
    }

    private static void RegisterOptions(
        IServiceCollection services,
        AuditLogEntityFrameworkCoreBuilder builder)
    {
        services.Configure<AuditLogEntityFrameworkCoreOptions>(opt =>
        {
            opt.Mode = builder.Mode;
            opt.ModelConfigurator = builder.ModelConfigurator;
            opt.AutoTransactionBehavior = builder.AutoTransactionBehavior;
        });
    }
}

/// <summary>
/// Builder for configuring Audit Log Entity Framework Core options elegantly.
/// </summary>
public sealed class AuditLogEntityFrameworkCoreBuilder
{
    public CAuditLogStoreMode Mode { get; private set; } = CAuditLogStoreMode.All;
    public Microsoft.EntityFrameworkCore.AutoTransactionBehavior? AutoTransactionBehavior { get; private set; }
    internal Action<LhaDbContextOptions>? DbContextConfig { get; private set; }
    internal Action<Microsoft.EntityFrameworkCore.ModelBuilder>? ModelConfigurator { get; private set; }

    /// <summary>
    /// Explicitly sets the audit store modes using flags.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UseMode(CAuditLogStoreMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    /// Enables both Data Audit (Entity changes) and Pipeline (High-throughput API requests) storage.
    /// This is the default mode.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UseAll()
    {
        Mode = CAuditLogStoreMode.All;
        return this;
    }

    /// <summary>
    /// Replaces the default audit settings to only use the high-throughput Pipeline dispatcher.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UsePipelineOnly()
    {
        Mode = CAuditLogStoreMode.Pipeline;
        return this;
    }

    /// <summary>
    /// Replaces the default audit settings to only use the relational Data Audit logging.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UseDataAuditOnly()
    {
        Mode = CAuditLogStoreMode.DataAudit;
        return this;
    }

    public AuditLogEntityFrameworkCoreBuilder ConfigureModel(Action<Microsoft.EntityFrameworkCore.ModelBuilder> configurator)
    {
        ModelConfigurator = configurator;
        return this;
    }

    /// <summary>
    /// Configures the EF Core DbContext options for the Audit Log module.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder ConfigureDbContext(Action<LhaDbContextOptions> configureOptions)
    {
        DbContextConfig = configureOptions;
        return this;
    }

    /// <summary>
    /// Sets the automatic transaction behavior for the Audit Log DbContext.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder SetAutoTransactionBehavior(Microsoft.EntityFrameworkCore.AutoTransactionBehavior behavior)
    {
        AutoTransactionBehavior = behavior;
        return this;
    }
}
