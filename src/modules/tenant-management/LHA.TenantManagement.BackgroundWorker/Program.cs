using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.TenantManagement.Application;
using LHA.TenantManagement.BackgroundWorker;
using LHA.TenantManagement.EntityFrameworkCore;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddTenantManagementApplication();
builder.Services.AddTenantManagementEntityFrameworkCore(options =>
{
    options.Configure<TenantManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Background workers ───────────────────────────────────────────
builder.Services.AddHostedService<TenantCleanupWorker>();

var host = builder.Build();
host.Run();
