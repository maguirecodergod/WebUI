using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.EventBus;
using LHA.Identity.EntityFrameworkCore;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Account.EntityFrameworkCore;

/// <summary>
/// Aggregates all module EF Core registrations for the Account Service.
/// Uses a unified <see cref="AccountDbContext"/> via the ReplaceDbContext pattern —
/// all module repositories share a single DbContext and transaction within each Unit of Work.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the unified <see cref="AccountDbContext"/> and all module EF Core services
    /// against a shared connection string.
    /// </summary>
    public static IServiceCollection AddAccountEntityFrameworkCore(
        this IServiceCollection services,
        string connectionString)
    {
        // 1) Register the unified AccountDbContext and replacement mappings.
        services.AddLhaDbContext<AccountDbContext>(options =>
        {
            options.Configure<AccountDbContext>(ctx =>
                ctx.DbContextOptions.UseNpgsql(connectionString));

            options.ReplaceDbContext<IdentityDbContext, AccountDbContext>();
            options.ReplaceDbContext<TenantManagementDbContext, AccountDbContext>();
            options.ReplaceDbContext<AuditLogDbContext, AccountDbContext>();
            options.ReplaceDbContext<PermissionManagementDbContext, AccountDbContext>();
        });

        // 2) Register module repositories, domain services, and per-module DbContexts.
        //    The module DbContexts are still registered in DI (used by EfCoreAuditingStore,
        //    EfCoreTenantStore, etc. which bypass UoW), but within a UoW the replacement
        //    map routes all providers to AccountDbContext.
        services.AddIdentityEntityFrameworkCore(options =>
        {
            options.Configure<IdentityDbContext>(ctx =>
                ctx.DbContextOptions.UseNpgsql(connectionString));
        });

        services.AddTenantManagementEntityFrameworkCore(options =>
        {
            options.Configure<TenantManagementDbContext>(ctx =>
                ctx.DbContextOptions.UseNpgsql(connectionString));
        });

        services.AddAuditLogEntityFrameworkCore(options =>
        {
            options.Configure<AuditLogDbContext>(ctx =>
                ctx.DbContextOptions.UseNpgsql(connectionString));
        });

        services.AddPermissionManagementEntityFrameworkCore(options =>
        {
            options.Configure<PermissionManagementDbContext>(ctx =>
                ctx.DbContextOptions.UseNpgsql(connectionString));
        });

        // Re-register outbox/inbox stores for the unified AccountDbContext.
        // Module DbContext registrations above each call TryAddEventStores,
        // and the last Replace() wins — override back to AccountDbContext.
        services.Replace(ServiceDescriptor.Scoped(typeof(IOutboxStore), typeof(EfCoreOutboxStore<AccountDbContext>)));
        services.Replace(ServiceDescriptor.Scoped(typeof(IInboxStore), typeof(EfCoreInboxStore<AccountDbContext>)));

        return services;
    }
}
