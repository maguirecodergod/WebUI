#pragma warning disable
#pragma warning restore

using LHA.Auditing;
using LHA.Auditing.Pipeline;
using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Registers EntityFrameworkCore infrastructure for the Audit Log module.
/// </summary>
public static class AuditLogEntityFrameworkCoreDependencyInjection
{
    /// <summary>
    /// Registers the <see cref="AuditLogDbContext"/>, repository, and audit stores
    /// backed by EF Core. Use the builder configuration to specify which audit modes to enable.
    /// </summary>
    public static IServiceCollection AddAuditLogEntityFrameworkCore(
        this IServiceCollection services,
        Action<AuditLogEntityFrameworkCoreBuilder>? configure = null)
    {
        var builder = new AuditLogEntityFrameworkCoreBuilder();
        configure?.Invoke(builder);

        services.Configure<AuditLogEntityFrameworkCoreOptions>(opt => opt.Mode = builder.Mode);

        // Always register the single shared DbContext
        services.AddLhaDbContext<AuditLogDbContext>(builder.DbContextConfig);

        if (builder.Mode.HasFlag(AuditLogStoreMode.DataAudit))
        {
            // Repository for querying/managing structured audit logs
            services.AddEfCoreRepository<AuditLogDbContext, AuditLogEntity, Guid>();
            services.TryAddScoped<IAuditLogRepository, EfCoreAuditLogRepository>();

            // Replaces LoggingAuditingStore so structured logs go to the DB
            services.Replace(ServiceDescriptor.Scoped<IAuditingStore, EfCoreAuditingStore>());
        }

        if (builder.Mode.HasFlag(AuditLogStoreMode.Pipeline))
        {
            // Replaces the default (logging) pipeline dispatcher with EF Core bulk insert
            services.AddSingleton<IAuditLogDispatcher, EfCoreAuditLogDispatcher>();
        }

        return services;
    }
}

/// <summary>
/// Builder for configuring AuditLog Entity Framework Core options elegantly.
/// </summary>
public sealed class AuditLogEntityFrameworkCoreBuilder
{
    internal AuditLogStoreMode Mode { get; private set; } = AuditLogStoreMode.All;
    internal Action<LhaDbContextOptions>? DbContextConfig { get; private set; }

    /// <summary>
    /// Explicitly sets the audit store modes using flags.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UseMode(AuditLogStoreMode mode)
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
        Mode = AuditLogStoreMode.All;
        return this;
    }

    /// <summary>
    /// Replaces the default audit settings to only use the high-throughput Pipeline dispatcher.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UsePipelineOnly()
    {
        Mode = AuditLogStoreMode.Pipeline;
        return this;
    }

    /// <summary>
    /// Replaces the default audit settings to only use the relational Data Audit logging.
    /// </summary>
    public AuditLogEntityFrameworkCoreBuilder UseDataAuditOnly()
    {
        Mode = AuditLogStoreMode.DataAudit;
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
}

/// <summary>
/// Controls which audit subsystems are wired to EF Core storage.
/// </summary>
[Flags]
public enum AuditLogStoreMode
{
    /// <summary>Structured relational audit log (AuditLog / Action / EntityChange tables).</summary>
    DataAudit = 1,

    /// <summary>High-throughput pipeline audit log (AuditLogPipeline table).</summary>
    Pipeline = 2,

    /// <summary>Both subsystems active simultaneously.</summary>
    All = DataAudit | Pipeline
}

/// <summary>
/// Holds runtime configuration for the AuditLog EntityFrameworkCore module.
/// </summary>
public sealed class AuditLogEntityFrameworkCoreOptions
{
    public AuditLogStoreMode Mode { get; set; } = AuditLogStoreMode.All;
}
