using LHA.Core.Users;
using LHA.TenantManagement.Domain;
using LHA.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LHA.Account.Migrator.Seeding;

/// <summary>
/// Seeds the default system tenant if it does not already exist.
/// </summary>
internal sealed class TenantSeeder : IDataSeeder
{
    private readonly ILogger _logger;

    public TenantSeeder(ILogger logger) => _logger = logger;

    public async Task SeedAsync(IServiceProvider serviceProvider, SeedingContext context)
    {
        var uowManager    = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var tenantManager = serviceProvider.GetRequiredService<TenantManager>();
        var tenantRepo    = serviceProvider.GetRequiredService<ITenantRepository>();

        using var uow = uowManager.Begin(isTransactional: true);

        var existing = await tenantRepo.FindByNameAsync(
            CurrentUserDefaults.DefaultTenantName.ToUpperInvariant());

        if (existing is null)
        {
            var tenant = await tenantManager.CreateAsync(
                CurrentUserDefaults.DefaultTenantName,
                CurrentUserDefaults.DefaultTenantId);

            await tenantRepo.InsertAsync(tenant);
            _logger.LogInformation(
                "Default tenant '{TenantName}' created (ID: {TenantId}).",
                tenant.Name, tenant.Id);
        }
        else
        {
            _logger.LogInformation("Default tenant already exists, skipping.");
        }

        await uow.CompleteAsync();
    }
}
