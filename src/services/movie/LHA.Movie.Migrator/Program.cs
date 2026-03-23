// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using LHA.Auditing;
using LHA.Grpc.Contracts.Services.Account.V1;
// using LHA.Movie.Application;
// using LHA.Movie.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditing();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();

// ── Module services (Application + EF Core) ──────────────────────
// builder.Services.AddMovieApplication();
// builder.Services.AddMovieEntityFrameworkCore(options =>
// {
//     options.Configure<MovieDbContext>(ctx =>
//         ctx.DbContextOptions.UseNpgsql(connectionString));
// });

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Movie.Migrator");

// ══════════════════════════════════════════════════════════════════
// 1. Apply pending migrations
// ══════════════════════════════════════════════════════════════════
logger.LogInformation("Applying Movie Service migrations...");

using (var scope = host.Services.CreateScope())
{
    // var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    // await dbContext.Database.MigrateAsync();
}

logger.LogInformation("Movie Service migrations applied successfully.");

// ══════════════════════════════════════════════════════════════════
// 2. Register permissions with Account Service via gRPC
// ══════════════════════════════════════════════════════════════════
var accountGrpcUrl = builder.Configuration["AccountService:GrpcUrl"]
    ?? "https://localhost:8150";

logger.LogInformation("Registering Movie permissions with Account Service at {Url} (gRPC)...", accountGrpcUrl);

using var channel = GrpcChannel.ForAddress(accountGrpcUrl);
var client = new PermissionRegistrationService.PermissionRegistrationServiceClient(channel);

var request = new RegisterPermissionsRequest
{
    ServiceName = "Movie",
    GrantAllToAdminRole = true,
};

request.Permissions.AddRange(
[
    new PermissionDefinitionDto { Name = "mega.accounts.read",   DisplayName = "View Movie Accounts",   GroupName = "MovieAccountManagement" },
    new PermissionDefinitionDto { Name = "mega.accounts.create", DisplayName = "Create Movie Accounts", GroupName = "MovieAccountManagement" },
    new PermissionDefinitionDto { Name = "mega.accounts.update", DisplayName = "Update Movie Accounts", GroupName = "MovieAccountManagement" },
    new PermissionDefinitionDto { Name = "mega.accounts.delete", DisplayName = "Delete Movie Accounts", GroupName = "MovieAccountManagement" },
]);

request.Groups.Add(new PermissionGroupDto
{
    Name = "MovieAccountManagement",
    DisplayName = "Movie Account Management",
});

var response = await client.RegisterPermissionsAsync(request);

logger.LogInformation("Movie permissions registered: {Success} – {Message}", response.Success, response.Message);

