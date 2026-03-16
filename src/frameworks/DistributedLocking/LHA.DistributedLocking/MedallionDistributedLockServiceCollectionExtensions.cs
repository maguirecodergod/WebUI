using LHA.DistributedLocking;
using LHA.DistributedLocking.Medallion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MedallionLocking = Medallion.Threading;

namespace LHA.DistributedLocking;

/// <summary>
/// Extension methods for registering Medallion-based distributed locking.
/// </summary>
public static class MedallionDistributedLockServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Medallion distributed lock adapter, replacing any previously
    /// registered <see cref="IDistributedLock"/>.
    /// <para>
    /// The caller must separately register a Medallion
    /// <see cref="MedallionLocking.IDistributedLockProvider"/> implementation
    /// (e.g., via <c>DistributedLock.Redis</c>, <c>DistributedLock.Postgres</c>,
    /// or <c>DistributedLock.SqlServer</c>).
    /// </para>
    /// </summary>
    public static IServiceCollection AddLHAMedallionDistributedLocking(
        this IServiceCollection services,
        Action<DistributedLockOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Ensure base abstractions (key normalizer, options) are registered
        services.AddLHADistributedLocking(configureOptions);

        // Replace the default in-process lock with the Medallion adapter
        services.Replace(ServiceDescriptor.Singleton<IDistributedLock, MedallionDistributedLock>());
        services.TryAddSingleton<IDistributedLockHealthCheck, MedallionDistributedLockHealthCheck>();

        return services;
    }
}
