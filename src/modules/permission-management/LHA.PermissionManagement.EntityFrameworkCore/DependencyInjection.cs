using LHA.EntityFrameworkCore;
using LHA.PermissionManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.PermissionManagement.EntityFrameworkCore;

public static class PermissionManagementEntityFrameworkCoreDependencyInjection
{
    public static IServiceCollection AddPermissionManagementEntityFrameworkCore(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
    {
        services.AddLhaDbContext<PermissionManagementDbContext>(configureOptions);

        // Generic repositories
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionDefinition, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionGroup, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionTemplate, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionGrant, Guid>();

        // Custom repositories
        services.TryAddScoped<IPermissionDefinitionRepository, EfCorePermissionDefinitionRepository>();
        services.TryAddScoped<IPermissionGroupRepository, EfCorePermissionGroupRepository>();
        services.TryAddScoped<IPermissionTemplateRepository, EfCorePermissionTemplateRepository>();
        services.TryAddScoped<IPermissionGrantRepository, EfCorePermissionGrantRepository>();

        return services;
    }
}
