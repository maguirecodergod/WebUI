using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.BackgroundWorker;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
// using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;

var builder = Host.CreateApplicationBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("Default")
//     ?? throw new InvalidOperationException("Missing 'Default' connection string.");

var mongoConnectionString = builder.Configuration.GetConnectionString("Mongo");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddAuditLogApplication();
var auditBuilder = new AuditLogEntityFrameworkCoreBuilder();
auditBuilder.UseAll();

// ─── POSTGRESQL CONFIGURATION ───
// auditBuilder.UsePostgreSql();
// auditBuilder.ConfigureDbContext(options =>
// {
//     options.Configure<AuditLogDbContext>(ctx =>
//         ctx.DbContextOptions.UseNpgsql(connectionString));
// });

// ─── MONGODB CONFIGURATION (Uncomment to use) ───
// Note: Requires referencing LHA.AuditLog.EntityFrameworkCore.MongoDB project
if (!string.IsNullOrEmpty(mongoConnectionString))
{
    auditBuilder.UseMongoDb();
    auditBuilder.ConfigureDbContext(options =>
    {
        options.Configure<AuditLogDbContext>(ctx =>
            ctx.DbContextOptions.UseMongoDB(mongoConnectionString, "LienHoaApp_AuditLog"));
    });
}
AuditLogEntityFrameworkCoreDependencyInjection.Register(builder.Services, auditBuilder);

// ── Background workers ───────────────────────────────────────────
builder.Services.AddHostedService<AuditLogCleanupWorker>();

using var host = builder.Build();
await host.RunAsync();
