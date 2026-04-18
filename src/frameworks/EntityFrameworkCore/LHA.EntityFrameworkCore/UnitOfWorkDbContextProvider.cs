using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// <see cref="IDbContextProvider{TDbContext}"/> implementation that integrates with
/// the <see cref="IUnitOfWork"/> system. It ensures that a single DbContext
/// instance is shared within the same Unit of Work boundary, and optionally participates
/// in an explicit database transaction.
/// <para>
/// Supports the ReplaceDbContext pattern: if <typeparamref name="TDbContext"/> has a replacement
/// registered in <see cref="LhaDbContextOptions.DbContextReplacements"/>, the replacement type
/// is resolved instead. All module providers that share the same replacement type will receive
/// the same DbContext instance within a Unit of Work.
/// </para>
/// <para>
/// Supports multi-tenant connection string resolution: if the current tenant has a dedicated
/// connection string (e.g., PerTenant database mode), the DbContext's connection is switched
/// to the tenant's database before first use.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The logical DbContext type to resolve.</typeparam>
public sealed class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
    where TDbContext : DbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly LhaDbContextOptions _options;

    private const string DbContextKey = "DbContext_";

    public UnitOfWorkDbContextProvider(
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        IOptions<LhaDbContextOptions> options)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    /// <inheritdoc />
    public async Task<DbContext> GetDbContextAsync()
    {
        var unitOfWork = _unitOfWorkManager.Current
            ?? throw new InvalidOperationException(
                $"A DbContext of type '{typeof(TDbContext).Name}' can only be created within an active unit of work.");

        // Resolve the actual type — may be a replacement (unified) DbContext.
        var actualType = _options.DbContextReplacements.GetValueOrDefault(
            typeof(TDbContext), typeof(TDbContext));

        // Include tenant ID in the UoW key so each tenant gets its own DbContext instance.
        // This is critical for PerTenant database mode where each tenant connects to a different database.
        var currentTenant = unitOfWork.ServiceProvider.GetService<ICurrentTenant>();
        var tenantKey = currentTenant?.Id?.ToString() ?? "host";
        var dbContextKey = DbContextKey + actualType.FullName + "_" + tenantKey;

        var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);
        if (databaseApi is EfCoreDatabaseApi existingApi)
        {
            return existingApi.DbContext;
        }

        var dbContext = await CreateDbContextAsync(unitOfWork, actualType, currentTenant);
        unitOfWork.AddDatabaseApi(dbContextKey, new EfCoreDatabaseApi(dbContext));

        if (dbContext is IHasCurrentUnitOfWork hasUow)
        {
            hasUow.CurrentUnitOfWork = unitOfWork;
        }

        if (unitOfWork.Options.IsTransactional)
        {
            await JoinTransactionAsync(unitOfWork, dbContext, dbContextKey);
        }

        return dbContext;
    }

    /// <summary>
    /// Creates a new DbContext, optionally switching its connection to the tenant's
    /// dedicated database if the current tenant has a custom connection string.
    /// </summary>
    private static async Task<DbContext> CreateDbContextAsync(
        IUnitOfWork unitOfWork, Type actualType, ICurrentTenant? currentTenant)
    {
        var dbContext = (DbContext)unitOfWork.ServiceProvider.GetRequiredService(actualType);

        // ── Multi-Tenant Connection Resolution ──────────────────────────────
        // If the current tenant has a dedicated connection string (PerTenant mode),
        // switch the DbContext to use that connection string instead of the default.
        if (currentTenant is { IsAvailable: true })
        {
            var tenantStore = unitOfWork.ServiceProvider.GetService<ITenantStore>();
            if (tenantStore is not null)
            {
                var tenantConfig = await tenantStore.FindAsync(currentTenant.Id!.Value);
                if (tenantConfig is not null)
                {
                    // Check for a "Default" connection string override
                    if (tenantConfig.ConnectionStrings.TryGetValue("Default", out var tenantConnectionString)
                        && !string.IsNullOrWhiteSpace(tenantConnectionString))
                    {
                        dbContext.Database.SetConnectionString(tenantConnectionString);
                    }
                }
            }
        }

        return dbContext;
    }

    private static async Task JoinTransactionAsync(IUnitOfWork unitOfWork, DbContext dbContext, string dbContextKey)
    {
        var transactionKey = DbContextKey + "Transaction_" + dbContextKey;

        var existingTransactionApi = unitOfWork.FindTransactionApi(transactionKey);
        if (existingTransactionApi is EfCoreTransactionApi efCoreTransactionApi)
        {
            // This DbContext should join the existing transaction.
            var dbTransaction = efCoreTransactionApi.Transaction.GetDbTransaction();
            await dbContext.Database.UseTransactionAsync(dbTransaction);
            efCoreTransactionApi.AttendedDbContexts.Add(dbContext);
        }
        else
        {
            // Start a new transaction for this DbContext.
            var isolationLevel = unitOfWork.Options.IsolationLevel;
            var transaction = isolationLevel.HasValue
                ? await dbContext.Database.BeginTransactionAsync(isolationLevel.Value)
                : await dbContext.Database.BeginTransactionAsync();

            unitOfWork.AddTransactionApi(
                transactionKey,
                new EfCoreTransactionApi(transaction, dbContext));
        }
    }
}
