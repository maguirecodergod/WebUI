using LHA.Auditing.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Auditing.Kafka;

/// <summary>
/// DI extensions for registering the Kafka audit log dispatcher.
/// </summary>
public static class KafkaAuditLogServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Kafka <see cref="IAuditLogDispatcher"/> as the primary
    /// audit log transport. Call after <c>AddLHAAuditPipeline()</c>.
    /// </summary>
    public static IServiceCollection AddLHAAuditKafkaDispatcher(
        this IServiceCollection services,
        Action<KafkaAuditLogOptions>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }

        // Replace the default logging dispatcher with Kafka
        services.AddSingleton<IAuditLogDispatcher, KafkaAuditLogDispatcher>();

        return services;
    }
}
