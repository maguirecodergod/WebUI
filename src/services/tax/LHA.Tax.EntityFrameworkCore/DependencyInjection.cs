using LHA.AuditLog.Domain;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.EntityFrameworkCore;
using LHA.EntityFrameworkCore.Auditing;
using LHA.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LHA.Tax.EntityFrameworkCore
{
    /// <summary>
    /// Aggregates all module EF Core registrations for the Tax Service.
    /// Uses a unified <see cref="TaxDbContext"/> via the ReplaceDbContext pattern —
    /// all module repositories share a single DbContext and transaction within each Unit of Work.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the unified <see ref="AccountDbContext"/> and all module EF Core services
        /// against a shared connection string.
        /// </summary>
        public static IServiceCollection AddTaxEntityFrameworkCore(
            this IServiceCollection services,
            string connectionString)
        {
            // 0) Register Data Auditing Interceptor
            services.AddScoped<DataAuditingSaveChangesInterceptor>();

            // 1) Register the unified TaxDbContext and replacement mappings.
            services.AddLhaDbContext<TaxDbContext>(options =>
            {
                options.Configure<TaxDbContext>(ctx =>
                    ctx.DbContextOptions.UseNpgsql(connectionString,
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

                options.ReplaceDbContext<AuditLogDbContext, TaxDbContext>();
            });

            // 2) Register module repositories, domain services, and per-module DbContexts.
            //    The module DbContexts are still registered in DI (used by EfCoreAuditingStore,
            //    EfCoreTenantStore, etc. which bypass UoW), but within a UoW the replacement
            //    map routes all providers to TaxDbContext.
            var auditBuilder = new AuditLogEntityFrameworkCoreBuilder();
            auditBuilder.UsePostgreSql();
            auditBuilder.UseAll();

            auditBuilder.ConfigureDbContext(options =>
            {
                options.Configure<AuditLogDbContext>(ctx =>
                    ctx.DbContextOptions.UseNpgsql(connectionString,
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            });

            AuditLogEntityFrameworkCoreDependencyInjection.Register(services, auditBuilder);

            // Re-register outbox/inbox stores for the unified AccountDbContext.
            // Module DbContext registrations above each call TryAddEventStores,
            // and the last Replace() wins — override back to TaxDbContext.
            services.Replace(ServiceDescriptor.Scoped(typeof(IOutboxStore), typeof(EfCoreOutboxStore<TaxDbContext>)));
            services.Replace(ServiceDescriptor.Scoped(typeof(IInboxStore), typeof(EfCoreInboxStore<TaxDbContext>)));

            // 3) Register Tax specialized Audit Log Repositories
            // services.AddScoped<IAuditLogRepository, EfCoreAuditLogRepository>();
            // services.AddScoped<IAuditLogActionRepository, EfCoreAuditLogActionRepository>();
            // services.AddScoped<IEntityChangeRepository, EfCoreEntityChangeRepository>();
            // services.AddScoped<IEntityPropertyChangeRepository, EfCoreEntityPropertyChangeRepository>();

            // 4) Register the Tenant Provisioning Migrator for TaxDbContext
            services.AddTransient<LHA.MultiTenancy.Provisioning.ITenantDatabaseMigrator, LHA.EntityFrameworkCore.Provisioning.EfCoreTenantDatabaseMigrator<TaxDbContext>>();

            return services;
        }
    }
}