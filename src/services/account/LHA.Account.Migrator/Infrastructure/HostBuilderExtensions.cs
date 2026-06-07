using LHA.Account.Application;
using LHA.Account.EntityFrameworkCore;
using LHA.Auditing;
using LHA.Caching;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Identity.Domain.Shared.Localization;
using LHA.Account.Domain.Shared.Localization;
using LHA.Localization;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Account.Migrator.Infrastructure;

/// <summary>
/// Extension methods for configuring the <see cref="IHostApplicationBuilder"/>
/// with all services required by the Account Migrator.
/// </summary>
internal static class HostBuilderExtensions
{
    /// <summary>
    /// Registers all framework, module, and localization services needed for migration and seeding.
    /// </summary>
    public static void AddAccountMigratorServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing 'Default' connection string.");

        // Framework services
        builder.Services.AddLHAAuditLogging();
        builder.Services.AddLHACaching();
        builder.Services.AddLHAMultiTenancy();
        builder.Services.AddLHAUnitOfWork();
        builder.Services.AddLHAInMemoryEventBus();
        builder.Services.AddLHADistributedLocking();
        builder.Services.AddSingleton<IClientInfoProvider, NullClientInfoProvider>();

        // Localization
        builder.Services.AddLHALocalization(options =>
        {
            options.AddResource<IdentityResource>();
            options.AddResource<AccountResource>();
        });

        // Module services
        builder.Services.AddAccountApplication();
        builder.Services.AddAccountEntityFrameworkCore(connectionString);
    }
}
