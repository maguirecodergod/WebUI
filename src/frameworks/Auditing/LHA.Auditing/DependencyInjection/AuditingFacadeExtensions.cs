using LHA.Auditing.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LHA.Auditing.Extensions;

namespace LHA.Auditing;

/// <summary>
/// Mode defining which auditing mechanisms (Producers) to enable across the pipeline.
/// </summary>
[Flags]
public enum AuditingMode
{
    /// <summary>Relational Data Action and Entity Logs</summary>
    DataAudit = 1,

    /// <summary>High-throughput API logs pipeline</summary>
    Pipeline = 2,

    /// <summary>Both auditing mechanisms</summary>
    All = DataAudit | Pipeline
}

/// <summary>
/// Unified facade for registering the Audit Logging mechanisms.
/// </summary>
public static class AuditingFacadeExtensions
{
    /// <summary>
    /// Registers both Data Auditing and the high-throughput Pipeline auditing 
    /// depending on the selected <paramref name="mode"/>.
    /// </summary>
    public static IServiceCollection AddLHAAuditLogging(
        this IServiceCollection services,
        AuditingMode mode = AuditingMode.All,
        Action<AuditingOptions>? configureDataAudit = null,
        Action<AuditPipelineOptions>? configurePipeline = null)
    {
        if (mode.HasFlag(AuditingMode.DataAudit))
        {
            services.AddLHAAuditing(configureDataAudit);
            services.AddAuditingInterception();
        }

        if (mode.HasFlag(AuditingMode.Pipeline))
        {
            services.AddLHAAuditPipeline(configurePipeline);
        }

        return services;
    }

    /// <summary>
    /// Configures the corresponding audit middlewares into the HTTP pipeline
    /// depending on the selected <paramref name="mode"/>.
    /// </summary>
    public static WebApplication UseLHAAuditLogging(
        this WebApplication app, 
        AuditingMode mode = AuditingMode.All)
    {
        if (mode.HasFlag(AuditingMode.DataAudit))
        {
            app.UseLHADataAuditing();
        }

        if (mode.HasFlag(AuditingMode.Pipeline))
        {
            app.UseLHAAuditPipeline();
        }

        return app;
    }
}
