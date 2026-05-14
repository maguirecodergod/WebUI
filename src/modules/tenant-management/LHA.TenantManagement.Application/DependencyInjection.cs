using LHA.Ddd.Domain;
using LHA.EventBus;
using LHA.TenantManagement.Application.Contracts;
using LHA.TenantManagement.Application.DomainEventHandlers;
using LHA.TenantManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.TenantManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTenantManagementApplication(this IServiceCollection services)
    {
        services.TryAddScoped<ITenantAppService, TenantAppService>();
        services.TryAddTransient<TenantManager>();

        services.AddTransient<IEventHandler<TenantCreatedDomainEvent>, TenantCreatedDomainEventHandler>();
        services.AddTransient<IEventHandler<TenantNameChangedDomainEvent>, TenantNameChangedDomainEventHandler>();
        services.AddTransient<IEventHandler<TenantActivationChangedDomainEvent>, TenantActivationChangedDomainEventHandler>();
        services.AddTransient<IEventHandler<TenantConnectionStringChangedDomainEvent>, TenantConnectionStringChangedDomainEventHandler>();

        return services;
    }
}
