using LHA.AspNetCore;
using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.PermissionManagement.Application;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.PermissionManagement.HttpApi;
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
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAExceptionHandler();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddPermissionManagementApplication();
builder.Services.AddPermissionManagementEntityFrameworkCore(options =>
{
    options.Configure<PermissionManagementDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHAUnitOfWork();
app.UseLHASwagger();

// ── Endpoints ────────────────────────────────────────────────────
app.MapPermissionManagementEndpoints();

app.Run();
