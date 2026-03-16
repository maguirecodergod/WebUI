using LHA.Mega.Application.Account;
using LHA.Mega.Application.Contracts.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Mega.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMegaApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IMegaAccountAppService, MegaAccountAppService>();
        return services;
    }
}
