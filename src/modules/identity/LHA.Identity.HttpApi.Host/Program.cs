using System.Text;
using LHA.AspNetCore;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Localization;
using LHA.MultiTenancy;
using LHA.Swagger;
using LHA.Identity.Application;
using LHA.Identity.Domain.Shared.Localization;
using LHA.Identity.EntityFrameworkCore;
using LHA.Identity.HttpApi;
using LHA.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHADistributedLocking();
builder.Services.AddLHAInMemoryEventBus();

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddLHASwagger(builder.Configuration);


builder.Services.AddLHAAspNetCore(typeof(IdentityResource));

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

builder.Services.AddAuthorization();

// ── Module services ──────────────────────────────────────────────
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityEntityFrameworkCore(options =>
{
    options.Configure<IdentityDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────
// RequestLocalization MUST come before the exception handler so that
// CultureInfo.CurrentUICulture is already set when error messages are resolved.
app.UseRequestLocalization(opts =>
{
    opts.AddSupportedCultures("en", "vi");
    opts.AddSupportedUICultures("en", "vi");
    opts.SetDefaultCulture("en");
});

app.UseLHAAspNetCore();

app.UseLHASwagger();
app.UseAuthentication();
app.UseAuthorization();

// ── Endpoints ────────────────────────────────────────────────────
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapRoleEndpoints();
app.MapPermissionEndpoints();
app.MapClaimTypeEndpoints();
app.MapSecurityLogEndpoints();

app.Run();
