using LHA.AspNetCore;
using LHA.Auditing;
using LHA.AuditLog.Application;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.HttpApi;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAExceptionHandler();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddAuditLogApplication();
builder.Services.AddAuditLogEntityFrameworkCore(options =>
{
    options.Configure<AuditLogDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHAUnitOfWork();
app.UseLHASwagger();

// ── Endpoints ────────────────────────────────────────────────────
app.MapAuditLogEndpoints();

app.Run();
