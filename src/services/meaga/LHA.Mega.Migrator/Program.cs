using Grpc.Net.Client;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Grpc.Contracts.Services.Account.V1;
using LHA.Mega.Application;
using LHA.Mega.Application.Contracts.Permissions;
using LHA.Mega.EntityFrameworkCore;
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
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHAInMemoryEventBus();
builder.Services.AddLHADistributedLocking();

// ── Module services (Application + EF Core) ──────────────────────
builder.Services.AddMegaApplication();
builder.Services.AddMegaEntityFrameworkCore(options =>
{
    options.Configure<MegaDbContext>(ctx =>
        ctx.DbContextOptions.UseNpgsql(connectionString));
});

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Mega.Migrator");

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

try
{
    await ApplyMigrationsAsync(host.Services, logger, cancellation.Token);
    await RegisterPermissionsAsync(builder.Configuration, logger, cancellation.Token);
    logger.LogInformation("Mega migrator completed successfully.");
    return;
}
catch (OperationCanceledException)
{
    logger.LogWarning("Mega migrator canceled.");
    Environment.ExitCode = 2;
}
catch (Exception ex)
{
    logger.LogError(ex, "Mega migrator failed.");
    Environment.ExitCode = 1;
}

static async Task ApplyMigrationsAsync(
    IServiceProvider services,
    ILogger logger,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Applying Mega Service migrations...");

    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MegaDbContext>();

    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
    var pending = pendingMigrations.ToArray();

    if (pending.Length == 0)
    {
        logger.LogInformation("No pending migrations found for Mega Service.");
        return;
    }

    logger.LogInformation("Found {Count} pending migration(s): {Migrations}.",
        pending.Length, string.Join(", ", pending));

    await dbContext.Database.MigrateAsync(cancellationToken);
    logger.LogInformation("Mega Service migrations applied successfully.");
}

static async Task RegisterPermissionsAsync(
    IConfiguration configuration,
    ILogger logger,
    CancellationToken cancellationToken)
{
    var accountGrpcUrl = configuration["AccountService:GrpcUrl"] ?? "https://localhost:8150";
    logger.LogInformation("Registering Mega permissions with Account Service at {Url} (gRPC)...", accountGrpcUrl);

    using var channel = GrpcChannel.ForAddress(accountGrpcUrl);
    var client = new PermissionRegistrationService.PermissionRegistrationServiceClient(channel);
    var request = BuildPermissionRegistrationRequest();

    const int maxAttempts = 3;
    Exception? lastException = null;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var response = await client.RegisterPermissionsAsync(
                request,
                cancellationToken: cancellationToken);

            if (!response.Success)
            {
                throw new InvalidOperationException(
                    $"Account service rejected Mega permission registration: {response.Message}");
            }

            logger.LogInformation("Mega permissions registered successfully: {Message}", response.Message);
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            lastException = ex;
            var delay = TimeSpan.FromSeconds(attempt * 2);
            logger.LogWarning(ex,
                "Permission registration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds}s...",
                attempt, maxAttempts, delay.TotalSeconds);
            await Task.Delay(delay, cancellationToken);
        }
        catch (Exception ex)
        {
            lastException = ex;
            break;
        }
    }

    throw new InvalidOperationException(
        $"Unable to register Mega permissions after {maxAttempts} attempts.",
        lastException);
}

static RegisterPermissionsRequest BuildPermissionRegistrationRequest()
{
    var request = new RegisterPermissionsRequest
    {
        ServiceName = "Mega",
        GrantAllToAdminRole = true,
    };

    request.Permissions.AddRange(
    [
        new PermissionDefinitionDto
        {
            Name = MegaPermissions.MegaAccountManagement.Read,
            DisplayName = MegaPermissions.MegaAccountManagement.L.Read,
            GroupName = MegaPermissions.MegaAccountManagement.GroupName,
            MultiTenancySide = PermissionMultiTenancySide.Tenant,
        },
        new PermissionDefinitionDto
        {
            Name = MegaPermissions.MegaAccountManagement.Create,
            DisplayName = MegaPermissions.MegaAccountManagement.L.Create,
            GroupName = MegaPermissions.MegaAccountManagement.GroupName,
            MultiTenancySide = PermissionMultiTenancySide.Tenant,
        },
        new PermissionDefinitionDto
        {
            Name = MegaPermissions.MegaAccountManagement.Update,
            DisplayName = MegaPermissions.MegaAccountManagement.L.Update,
            GroupName = MegaPermissions.MegaAccountManagement.GroupName,
            MultiTenancySide = PermissionMultiTenancySide.Tenant,
        },
        new PermissionDefinitionDto
        {
            Name = MegaPermissions.MegaAccountManagement.Delete,
            DisplayName = MegaPermissions.MegaAccountManagement.L.Delete,
            GroupName = MegaPermissions.MegaAccountManagement.GroupName,
            MultiTenancySide = PermissionMultiTenancySide.Tenant,
        },
    ]);

    request.Groups.Add(new PermissionGroupDto
    {
        Name = MegaPermissions.MegaAccountManagement.GroupName,
        DisplayName = MegaPermissions.MegaAccountManagement.L.Group,
    });

    return request;
}

