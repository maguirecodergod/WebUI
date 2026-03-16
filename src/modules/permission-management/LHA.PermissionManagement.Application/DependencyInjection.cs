using LHA.PermissionManagement.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.PermissionManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPermissionManagementApplication(
        this IServiceCollection services)
    {
        services.TryAddScoped<IPermissionDefinitionAppService, PermissionDefinitionAppService>();
        services.TryAddScoped<IPermissionGroupAppService, PermissionGroupAppService>();
        services.TryAddScoped<IPermissionTemplateAppService, PermissionTemplateAppService>();
        services.TryAddScoped<IPermissionGrantAppService, PermissionGrantAppService>();

        return services;
    }
}
