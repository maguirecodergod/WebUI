using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Identity.Application;
using LHA.Identity.BackgroundWorker;
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

// ── Module services ──────────────────────────────────────────────
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityEntityFrameworkCore(options =>
{
    options.Configure<IdentityDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Background workers ───────────────────────────────────────────
builder.Services.AddHostedService<ExpiredTokenCleanupWorker>();
builder.Services.AddHostedService<SecurityLogCleanupWorker>();

var host = builder.Build();
host.Run();
