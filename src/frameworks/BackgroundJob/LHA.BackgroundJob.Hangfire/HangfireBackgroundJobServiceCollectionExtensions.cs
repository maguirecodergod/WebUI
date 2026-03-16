using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.BackgroundJob.Hangfire;

/// <summary>
/// Extension methods for registering the Hangfire-backed background job system.
/// </summary>
public static class HangfireBackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Hangfire <see cref="IBackgroundJobManager"/> implementation.
    /// Requires that Hangfire itself is configured separately
    /// (e.g. via <c>AddLHAHangfireScheduling</c> or <c>services.AddHangfire(...)</c>).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHAHangfireBackgroundJobs(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLHABackgroundJobs();

        // Replace the null manager with Hangfire
        services.AddSingleton<IBackgroundJobManager, HangfireBackgroundJobManager>();

        return services;
    }
}
