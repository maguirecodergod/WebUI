using LHA.AspNetCore;
using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.HttpApi;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
// using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("Default")
//     ?? throw new InvalidOperationException("Missing 'Default' connection string.");

var mongoConnectionString = builder.Configuration.GetConnectionString("Mongo");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAExceptionHandler();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddAuditLogApplication();
builder.Services.AddAuditLogEntityFrameworkCore(auditBuilder =>
{
    // 1. All Mode (Default): Uses both Relational Structured Logs & Pipeline Logs
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
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHAUnitOfWork();
app.UseLHASwagger();

// ── Endpoints ────────────────────────────────────────────────────
app.MapAuditLogEndpoints();

app.Run();
