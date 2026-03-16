using LHA.Auditing;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddAuditLogEntityFrameworkCore(options =>
{
    options.Configure<AuditLogDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("AuditLog.Migrator");

// ── 1. Apply pending migrations ──────────────────────────────────
logger.LogInformation("Applying AuditLog migrations...");

using (var migrationScope = host.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<AuditLogDbContext>();
    await dbContext.Database.MigrateAsync();
}

logger.LogInformation("AuditLog migrations applied successfully.");

// No seed data needed for audit logs — they are created by the auditing pipeline.
logger.LogInformation("AuditLog migration complete. No seed data required.");
