using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Auditing;

/// <summary>
/// Registers LHA auditing services in the DI container.
/// </summary>
public static class AuditingServiceCollectionExtensions
{
    /// <summary>
    /// Adds core LHA auditing services:
    /// <list type="bullet">
    ///   <item><see cref="IAuditingManager"/> — scope management with <see cref="AsyncLocal{T}"/></item>
    ///   <item><see cref="IAuditPropertySetter"/> — sets audit properties on entities</item>
    ///   <item><see cref="IAuditSerializer"/> — JSON serialization for parameters</item>
    ///   <item><see cref="IAuditingStore"/> — logging fallback (replace with your persistence)</item>
    ///   <item><see cref="IAuditUserProvider"/> — null provider (replace with your identity bridge)</item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddLHAAuditing(
        this IServiceCollection services,
        Action<AuditingOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<IAuditUserProvider, NullAuditUserProvider>();
        services.TryAddSingleton<IAuditingStore, LoggingAuditingStore>();
        services.TryAddSingleton<IAuditSerializer, JsonAuditSerializer>();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IAuditingManager, AuditingManager>();
        services.TryAddTransient<IAuditPropertySetter, AuditPropertySetter>();

        return services;
    }
}
