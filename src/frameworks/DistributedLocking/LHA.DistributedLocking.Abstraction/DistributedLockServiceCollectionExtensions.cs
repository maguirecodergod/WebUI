using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.DistributedLocking;

/// <summary>
/// Extension methods for registering distributed locking services.
/// </summary>
public static class DistributedLockServiceCollectionExtensions
{
    /// <summary>
    /// Registers the distributed lock abstraction with an in-process
    /// <see cref="LocalDistributedLock"/> as the default provider.
    /// </summary>
    public static IServiceCollection AddLHADistributedLocking(
        this IServiceCollection services,
        Action<DistributedLockOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }

        services.TryAddSingleton<IDistributedLockKeyNormalizer, DistributedLockKeyNormalizer>();
        services.TryAddSingleton<IDistributedLock, LocalDistributedLock>();

        return services;
    }
}
