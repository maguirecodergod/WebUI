using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Application;
using LHA.PermissionManagement.BackgroundWorker;
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

// ── Module services ──────────────────────────────────────────────
builder.Services.AddPermissionManagementApplication();
builder.Services.AddPermissionManagementEntityFrameworkCore(options =>
{
    options.Configure<PermissionManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

// ── Background workers ───────────────────────────────────────────
builder.Services.AddHostedService<PermissionGrantCleanupWorker>();

var host = builder.Build();
host.Run();
