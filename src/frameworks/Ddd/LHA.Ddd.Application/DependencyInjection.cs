using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Ddd.Application;

/// <summary>
/// Registers framework-level application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds core LHA DDD Application services.
    /// </summary>
    public static IServiceCollection AddLHADddApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IAuditedDtoEnricher, AuditedDtoEnricher>();
        return services;
    }
}
