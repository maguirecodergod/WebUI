using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Core.Users;

/// <summary>
/// Extension methods for registering <see cref="ICurrentUser"/> infrastructure.
/// </summary>
public static class UserServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core user context infrastructure:
    /// <list type="bullet">
    ///   <item><see cref="ICurrentUserAccessor"/> — AsyncLocal ambient storage (Singleton)</item>
    ///   <item><see cref="ICurrentUser"/> — Null fallback (Scoped, replaced by hosting layer)</item>
    /// </list>
    /// <para>
    /// Call this from framework bootstrap. The ASP.NET Core hosting layer
    /// will replace <see cref="ICurrentUser"/> with <c>HttpContextCurrentUser</c>.
    /// </para>
    /// </summary>
    public static IServiceCollection AddLHACurrentUser(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ICurrentUserAccessor, AsyncLocalCurrentUserAccessor>();
        services.TryAddScoped<ICurrentUser, NullCurrentUser>();

        return services;
    }
}
