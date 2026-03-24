using System.Text;
using LHA.Account.Application;
using LHA.Account.Domain.Shared.Localization;
using LHA.Account.EntityFrameworkCore;
using LHA.Account.HttpApi;
using LHA.AspNetCore;
using LHA.Auditing;
using LHA.Auditing.EfCore;
using LHA.Auditing.Extensions;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Grpc.Server;
using LHA.Identity.Application;
using LHA.Identity.Domain.Shared.Localization;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing(options => 
{
    options.ApplicationName = "Account";
});
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAInMemoryEventBus();

// ── Audit Pipeline (next-gen) ────────────────────────────────────
builder.Services.AddLHAAuditPipeline(options =>
{
    options.ServiceName = "Account";
    options.CaptureRequestBody = true;
    options.CaptureResponseBody = false;
    options.BatchSize = 500;
    options.FlushIntervalMs = 2_000;
    options.SamplingRate = 1.0;
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLhaApiVersioning();
builder.Services.AddLHASwagger(builder.Configuration);

// ── Global exception handler ──────────────────────────────────────
builder.Services.AddLHAAspNetCore(typeof(AccountResource), typeof(IdentityResource));

// ── JWT configuration ────────────────────────────────────────────
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Missing JWT SecretKey.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });

builder.Services.AddLHAPermissionAuthorization();

// ── gRPC server ───────────────────────────────────────────────────
builder.Services.AddLHAGrpcServer();

// ── Module services (Application + EF Core) ──────────────────────
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

builder.Services.AddAccountApplication();
builder.Services.AddAccountEntityFrameworkCore(connectionString);

// Configure EF Core dispatcher to store the audit records
builder.Services.AddLHAAuditEfCoreDispatcher(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
app.UseLHAExceptionHandler();
app.UseLHADataAuditing();
app.UseLHAUnitOfWork();

// Audit pipeline middleware — after exception handler, before auth
// app.UseLHAAuditLogging();
app.UseLHASwagger();
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ────────────────────────────────────────────────────
app.MapAccountEndpoints();
app.MapLHAGrpcInfrastructure();

app.Run();
