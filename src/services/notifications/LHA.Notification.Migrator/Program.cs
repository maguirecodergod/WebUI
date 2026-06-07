using Grpc.Net.Client;
using LHA.Auditing;
using LHA.DistributedLocking;
using LHA.EventBus;
using LHA.Grpc.Contracts.Services.Account.V1;
using LHA.Notification.Application.DependencyInjection;

using LHA.Notification.Infrastructure;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LHA.Notification.Infrastructure.Persistences;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'Default' connection string.");

// ── Framework services ───────────────────────────────────────────
builder.Services.AddLHAAuditLogging();
builder.Services.AddLHAMultiTenancy();
builder.Services.AddLHAUnitOfWork();
builder.Services.AddLHAInMemoryEventBus();
builder.Services.AddLHADistributedLocking();

// ── Module services (Application + EF Core) ──────────────────────
builder.Services.AddNotificationApplication();
builder.Services.AddNotificationInfrastructure(builder.Configuration);

using var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Notification.Migrator");

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
    logger.LogInformation("Notification migrator completed successfully.");
    return;
}
catch (OperationCanceledException)
{
    logger.LogWarning("Notification migrator canceled.");
    Environment.ExitCode = 2;
}
catch (Exception ex)
{
    logger.LogError(ex, "Notification migrator failed.");
    Environment.ExitCode = 1;
}

static async Task ApplyMigrationsAsync(
    IServiceProvider services,
    ILogger logger,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Applying Notification Service migrations...");

    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

    // MongoDB does not use relational migrations.
    // EnsureCreatedAsync creates the database and collections if they don't exist.
    await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    logger.LogInformation("Notification Service database ensured successfully.");
}

static async Task RegisterPermissionsAsync(
    IConfiguration configuration,
    ILogger logger,
    CancellationToken cancellationToken)
{
    var accountGrpcUrl = configuration["AccountService:GrpcUrl"] ?? "https://localhost:8150";
    logger.LogInformation("Registering Notification permissions with Account Service at {Url} (gRPC)...", accountGrpcUrl);

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
                    $"Account service rejected Notification permission registration: {response.Message}");
            }

            logger.LogInformation("Notification permissions registered successfully: {Message}", response.Message);
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
        $"Unable to register Notification permissions after {maxAttempts} attempts.",
        lastException);
}

static RegisterPermissionsRequest BuildPermissionRegistrationRequest()
{
    var request = new RegisterPermissionsRequest
    {
        ServiceName = "Notification",
        GrantAllToAdminRole = true,
    };

    request.Groups.Add(new PermissionGroupDto
    {
        Name = NotificationPermissions.GroupName,
        DisplayName = NotificationPermissions.L.Group,
    });

    request.Permissions.AddRange(
    [
        // Notifications
        new PermissionDefinitionDto { Name = NotificationPermissions.Notifications.Default, DisplayName = NotificationPermissions.Notifications.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Notifications.Send, DisplayName = NotificationPermissions.Notifications.L.Send, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Notifications.Read, DisplayName = NotificationPermissions.Notifications.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Notifications.Delete, DisplayName = NotificationPermissions.Notifications.L.Delete, GroupName = NotificationPermissions.GroupName },

        // Devices
        new PermissionDefinitionDto { Name = NotificationPermissions.Devices.Default, DisplayName = NotificationPermissions.Devices.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Devices.Read, DisplayName = NotificationPermissions.Devices.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Devices.Manage, DisplayName = NotificationPermissions.Devices.L.Manage, GroupName = NotificationPermissions.GroupName },

        // Templates
        new PermissionDefinitionDto { Name = NotificationPermissions.Templates.Default, DisplayName = NotificationPermissions.Templates.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Templates.Read, DisplayName = NotificationPermissions.Templates.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Templates.Create, DisplayName = NotificationPermissions.Templates.L.Create, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Templates.Update, DisplayName = NotificationPermissions.Templates.L.Update, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Templates.Delete, DisplayName = NotificationPermissions.Templates.L.Delete, GroupName = NotificationPermissions.GroupName },

        // Batches
        new PermissionDefinitionDto { Name = NotificationPermissions.Batches.Default, DisplayName = NotificationPermissions.Batches.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Batches.Read, DisplayName = NotificationPermissions.Batches.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Batches.Create, DisplayName = NotificationPermissions.Batches.L.Create, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Batches.Manage, DisplayName = NotificationPermissions.Batches.L.Manage, GroupName = NotificationPermissions.GroupName },

        // Preferences
        new PermissionDefinitionDto { Name = NotificationPermissions.Preferences.Default, DisplayName = NotificationPermissions.Preferences.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Preferences.Read, DisplayName = NotificationPermissions.Preferences.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Preferences.Update, DisplayName = NotificationPermissions.Preferences.L.Update, GroupName = NotificationPermissions.GroupName },

        // Configuration
        new PermissionDefinitionDto { Name = NotificationPermissions.Configuration.Default, DisplayName = NotificationPermissions.Configuration.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Configuration.Read, DisplayName = NotificationPermissions.Configuration.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Configuration.Manage, DisplayName = NotificationPermissions.Configuration.L.Manage, GroupName = NotificationPermissions.GroupName },

        // Statistics
        new PermissionDefinitionDto { Name = NotificationPermissions.Statistics.Default, DisplayName = NotificationPermissions.Statistics.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.Statistics.Read, DisplayName = NotificationPermissions.Statistics.L.Read, GroupName = NotificationPermissions.GroupName },

        // AuditLogs
        new PermissionDefinitionDto { Name = NotificationPermissions.AuditLogs.Default, DisplayName = NotificationPermissions.AuditLogs.L.Default, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.AuditLogs.Read, DisplayName = NotificationPermissions.AuditLogs.L.Read, GroupName = NotificationPermissions.GroupName },
        new PermissionDefinitionDto { Name = NotificationPermissions.AuditLogs.Delete, DisplayName = NotificationPermissions.AuditLogs.L.Delete, GroupName = NotificationPermissions.GroupName }
    ]);

    return request;
}
