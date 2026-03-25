using LHA.Auditing.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LHA.Auditing.Extensions;

namespace LHA.Auditing;
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
        CAuditingMode mode = CAuditingMode.All,
        Action<AuditingOptions>? configureDataAudit = null,
        Action<AuditPipelineOptions>? configurePipeline = null)
    {
        if (mode.HasFlag(CAuditingMode.DataAudit))
        {
            services.AddLHAAuditing(configureDataAudit);
            services.AddAuditingInterception();
        }

        if (mode.HasFlag(CAuditingMode.Pipeline))
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
        CAuditingMode mode = CAuditingMode.All)
    {
        if (mode.HasFlag(CAuditingMode.DataAudit))
        {
            app.UseLHADataAuditing();
        }

        if (mode.HasFlag(CAuditingMode.Pipeline))
        {
            app.UseLHAAuditPipeline();
        }

        return app;
    }
}
