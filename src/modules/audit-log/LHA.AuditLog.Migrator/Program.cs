using LHA.Auditing;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
// using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("Default")
//     ?? throw new InvalidOperationException("Missing 'Default' connection string.");

var mongoConnectionString = builder.Configuration.GetConnectionString("Mongo");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services ──────────────────────────────────────────────
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

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("AuditLog.Migrator");

// ── 1. Apply pending migrations ──────────────────────────────────
logger.LogInformation("Applying AuditLog migrations...");

using (var migrationScope = host.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<AuditLogDbContext>();
    if (dbContext.Database.IsRelational())
    {
        logger.LogInformation("Relational database detected. Applying pending migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("AuditLog migrations applied successfully.");
    }
    else
    {
        logger.LogInformation("Non-relational database detected (MongoDB). Ensuring collections are created...");
        await dbContext.Database.EnsureCreatedAsync();
        logger.LogInformation("AuditLog collections ensured successfully.");
    }
}

// No seed data needed for audit logs — they are created by the auditing pipeline.
logger.LogInformation("AuditLog initialization complete. No seed data required.");
