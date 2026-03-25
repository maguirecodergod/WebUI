using LHA.Auditing.Interceptors;
using LHA.Auditing.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Auditing.Extensions;

/// <summary>
/// Extension methods for registering the high-performance audit logging pipeline
/// in the DI container.
/// </summary>
internal static class AuditPipelineServiceCollectionExtensions
{
    /// <summary>
    /// Adds the high-performance audit logging pipeline:
    /// <list type="bullet">
    ///   <item><see cref="IAuditLogCollector"/> — enrichment + masking + sampling</item>
    ///   <item><see cref="IAuditLogBuffer"/> — bounded Channel-based buffer</item>
    ///   <item><see cref="AuditLogBatchProcessor"/> — background batch dispatcher</item>
    ///   <item><see cref="IAuditLogEnricher"/> — trace context + service info enrichers</item>
    ///   <item><see cref="IAuditDataMasker"/> — sensitive field masking</item>
    ///   <item><see cref="AuditPipelineMetrics"/> — OpenTelemetry metrics</item>
    ///   <item><see cref="AuditPipelineCircuitBreaker"/> — dispatch resilience</item>
    /// </list>
    /// </summary>
    internal static IServiceCollection AddLHAAuditPipeline(
        this IServiceCollection services,
        Action<AuditPipelineOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Configure options
        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Core pipeline (singletons — shared across requests)
        services.TryAddSingleton<AuditPipelineMetrics>();
        services.TryAddSingleton<AuditPipelineCircuitBreaker>();
        services.TryAddSingleton<IAuditLogBuffer, ChannelAuditLogBuffer>();
        services.TryAddSingleton<IAuditDataMasker, AuditDataMasker>();

        // Collector (scoped — needs ICurrentUser which is scoped)
        services.TryAddScoped<IAuditLogCollector, ChannelAuditLogCollector>();

        // Default dispatcher (logging — can be overridden by Kafka/EfCore)
        services.TryAddSingleton<IAuditLogDispatcher, LoggingAuditLogDispatcher>();

        // Enrichers
        services.AddSingleton<IAuditLogEnricher, TraceContextEnricher>();
        services.AddSingleton<IAuditLogEnricher, ServiceInfoEnricher>();

        // Background batch processor
        services.AddHostedService<AuditLogBatchProcessor>();

        return services;
    }

    /// <summary>
    /// Adds the audit logging HTTP middleware to the request pipeline.
    /// Call this early, before authentication/authorization.
    /// </summary>
    internal static WebApplication UseLHAAuditPipeline(this WebApplication app)
    {
        app.UseMiddleware<AuditLoggingMiddleware>();
        return app;
    }
}
