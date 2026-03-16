using Microsoft.Extensions.DependencyInjection;

namespace LHA.BackgroundJob.Quartz;

/// <summary>
/// Extension methods for registering the Quartz-backed background job system.
/// </summary>
public static class QuartzBackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Quartz <see cref="IBackgroundJobManager"/> implementation.
    /// Requires that Quartz.NET is configured separately
    /// (e.g. via <c>AddLHAQuartzScheduling</c> or <c>services.AddQuartz(...)</c>).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration for Quartz background job options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHAQuartzBackgroundJobs(
        this IServiceCollection services,
        Action<QuartzBackgroundJobOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLHABackgroundJobs();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Replace the null manager with Quartz
        services.AddSingleton<IBackgroundJobManager, QuartzBackgroundJobManager>();

        return services;
    }
}
