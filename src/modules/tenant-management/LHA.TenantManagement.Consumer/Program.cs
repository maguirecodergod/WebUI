using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.MultiTenancy;
using LHA.TenantManagement.Application;
using LHA.TenantManagement.Application.Contracts;
using LHA.TenantManagement.Consumer;
using LHA.TenantManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAInMemoryEventBus();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddTenantManagementApplication();
builder.Services.AddTenantManagementEntityFrameworkCore(options =>
{
    options.Configure<TenantManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Event handlers ───────────────────────────────────────────────
builder.Services.AddTransient<IEventHandler<TenantCreatedEto>, TenantCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<TenantActivationChangedEto>, TenantActivationChangedEventHandler>();
builder.Services.AddTransient<IEventHandler<TenantConnectionStringChangedEto>, TenantConnectionStringChangedEventHandler>();

var host = builder.Build();
host.Run();
