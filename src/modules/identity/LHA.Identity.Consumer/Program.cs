using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Identity.Application;
using LHA.Identity.Application.Contracts;
using LHA.Identity.Consumer;
using LHA.Identity.EntityFrameworkCore;
using LHA.MultiTenancy;
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
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityEntityFrameworkCore(options =>
{
    options.Configure<IdentityDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Event handlers ───────────────────────────────────────────────
builder.Services.AddTransient<IEventHandler<UserCreatedEto>, UserCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<UserRoleChangedEto>, UserRoleChangedEventHandler>();
builder.Services.AddTransient<IEventHandler<LoginFailedEto>, LoginFailedEventHandler>();

var host = builder.Build();
host.Run();
