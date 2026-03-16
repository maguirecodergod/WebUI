using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.BackgroundWorker;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
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
builder.Services.AddAuditLogApplication();
builder.Services.AddAuditLogEntityFrameworkCore(options =>
{
    options.Configure<AuditLogDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Background workers ───────────────────────────────────────────
builder.Services.AddHostedService<AuditLogCleanupWorker>();

using var host = builder.Build();
await host.RunAsync();
