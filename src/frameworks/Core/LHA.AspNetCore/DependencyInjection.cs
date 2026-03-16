using LHA.AspNetCore.Authorization;
using LHA.Auditing;
using LHA.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.AspNetCore;

/// <summary>
/// Extension methods for registering the LHA global exception handler
/// and other ASP.NET Core cross-cutting concerns.
/// </summary>
public static class LhaAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// Convenience method to add all core LHA ASP.NET Core services, including:
    /// <list type="bullet">
    /// <item><description><see cref="GlobalExceptionHandler"/> for consistent API error responses</description></item>
    /// <item><description><see cref="IClientInfoProvider"/> for audit/security log context</description></item>
    /// <item><description>Localization services for API responses and business exceptions</description></item>
    /// </list>
    /// <para>>
    /// The generic type parameter <typeparamref name="TResource"/> should be the root localization resource of the module (e.g. <c>IdentityResource</c> in the
    /// </summary>
    /// <typeparam name="TResource"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddLHAAspNetCore(
        this IServiceCollection services,
        params Type[] resources)
    {
        services.AddLHAExceptionHandler();

        services.AddLHALocalization(opts =>
        {
            foreach (var resource in resources)
            {
                opts.AddResource(resource);
            }
        });

        services.AddLHABusinessExceptionLocalization();

        return services;
    }

    /// <summary>
    /// Registers the <see cref="GlobalExceptionHandler"/> that converts all
    /// unhandled exceptions into a standardised <c>ApiResponse</c> JSON envelope.
    /// Also registers <see cref="IHttpContextAccessor"/> and
    /// <see cref="IClientInfoProvider"/> for audit/security log context.
    /// </summary>
    public static IServiceCollection AddLHAExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(); // fallback; won't be used since our handler returns true

        // Ensure HttpContext is available for client info extraction
        services.AddHttpContextAccessor();
        services.TryAddSingleton<IClientInfoProvider, HttpContextClientInfoProvider>();

        // Serialise all DateTimeOffset values as local time in API responses
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new LocalDateTimeOffsetJsonConverter());
            options.SerializerOptions.Converters.Add(new NullableLocalDateTimeOffsetJsonConverter());
        });

        return services;
    }
    /// <summary>
    /// Registers <see cref="BusinessExceptionLocalizer"/> as the
    /// <see cref="IBusinessExceptionLocalizer"/> used by <see cref="GlobalExceptionHandler"/>
    /// to resolve culture-aware messages for every <see cref="LHA.Ddd.Domain.BusinessException"/>.
    /// <para>
    /// Call this <b>after</b> <c>AddLHALocalization(…)</c> so that
    /// <c>ILHAStringLocalizerFactory</c> and <c>LocalizationResourceOptions</c>
    /// are already registered in the container.
    /// </para>
    /// </summary>
    public static IServiceCollection AddLHABusinessExceptionLocalization(
        this IServiceCollection services)
    {
        services.TryAddSingleton<IBusinessExceptionLocalizer, BusinessExceptionLocalizer>();
        return services;
    }

    /// <summary>
    /// Registers the permission-based authorization system:
    /// <see cref="PermissionAuthorizationPolicyProvider"/> dynamically creates policies,
    /// <see cref="PermissionAuthorizationHandler"/> evaluates JWT <c>permissions</c> claims.
    /// </summary>
    public static IServiceCollection AddLHAPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}

/// <summary>
/// Extension methods for configuring the LHA exception handling middleware.
/// </summary>
public static class LhaAspNetCoreApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the global exception handling middleware. Call this early in the pipeline,
    /// before authentication and routing.
    /// </summary>
    public static WebApplication UseLHAExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler();
        return app;
    }

    /// <summary>
    /// Adds the Unit-of-Work middleware that wraps every HTTP request in an ambient
    /// <see cref="LHA.UnitOfWork.IUnitOfWork"/>. Place this after exception handling
    /// but before authentication / routing.
    /// </summary>
    public static WebApplication UseLHAUnitOfWork(this WebApplication app)
    {
        app.UseMiddleware<UnitOfWorkMiddleware>();
        return app;
    }

    public static WebApplication UseLHAAspNetCore(this WebApplication app)
    {
        app.UseLHAExceptionHandler();
        app.UseLHAUnitOfWork();
        return app;
    }
}
