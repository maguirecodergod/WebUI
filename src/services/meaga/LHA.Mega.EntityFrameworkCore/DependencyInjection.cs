using LHA.EntityFrameworkCore;
using LHA.Mega.Domain.Account;
using LHA.Mega.EntityFrameworkCore.Account;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Mega.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddMegaEntityFrameworkCore(
        this IServiceCollection services,
        Action<LhaDbContextOptions>? configureOptions = null)
    {
        services.AddLhaDbContext<MegaDbContext>(configureOptions);

        services.AddEfCoreRepository<MegaDbContext, MegaAccountEntity, Guid>();
        services.TryAddScoped<IMegaAccountRepository, EfCoreMegaAccountRepository>();

        return services;
    }
}
