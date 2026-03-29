using LHA.EntityFrameworkCore;
using LHA.PermissionManagement.Domain;
using LHA.PermissionManagement.Domain.PermissionDefinitions;
using LHA.PermissionManagement.Domain.PermissionGrants;
using LHA.PermissionManagement.Domain.PermissionGroups;
using LHA.PermissionManagement.Domain.PermissionTemplates;
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
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionDefinitionEntity, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionGroupEntity, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionTemplateEntity, Guid>();
        services.AddEfCoreRepository<PermissionManagementDbContext, PermissionGrantEntity, Guid>();

        // Custom repositories
        services.TryAddScoped<IPermissionDefinitionRepository, EfCorePermissionDefinitionRepository>();
        services.TryAddScoped<IPermissionGroupRepository, EfCorePermissionGroupRepository>();
        services.TryAddScoped<IPermissionTemplateRepository, EfCorePermissionTemplateRepository>();
        services.TryAddScoped<IPermissionGrantRepository, EfCorePermissionGrantRepository>();

        return services;
    }
}
