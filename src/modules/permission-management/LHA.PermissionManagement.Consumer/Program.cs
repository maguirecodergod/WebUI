using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Application;
using LHA.PermissionManagement.Application.Contracts;
using LHA.PermissionManagement.Consumer;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAInMemoryEventBus();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddPermissionManagementApplication();
builder.Services.AddPermissionManagementEntityFrameworkCore(options =>
{
    options.Configure<PermissionManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Event handlers ───────────────────────────────────────────────
builder.Services.AddTransient<IEventHandler<PermissionGrantedEto>, PermissionGrantedEventHandler>();
builder.Services.AddTransient<IEventHandler<PermissionRevokedEto>, PermissionRevokedEventHandler>();

var host = builder.Build();
host.Run();
