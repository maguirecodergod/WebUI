using LHA.Grpc.Contracts.Services.Account.V1;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.HttpApi.Host.BackgroundJobs;

public class PermissionRegistrationHostedService : IHostedService
{
    private readonly PermissionRegistrationService.PermissionRegistrationServiceClient _client;
    private readonly ILogger<PermissionRegistrationHostedService> _logger;

    public PermissionRegistrationHostedService(
        PermissionRegistrationService.PermissionRegistrationServiceClient client,
        ILogger<PermissionRegistrationHostedService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering Notification permissions with Account Service (gRPC)...");

        try
        {
            var request = BuildPermissionRegistrationRequest();

            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var response = await _client.RegisterPermissionsAsync(request, cancellationToken: cancellationToken);
                    if (response.Success)
                    {
                        _logger.LogInformation("Notification permissions registered successfully: {Message}", response.Message);
                        return;
                    }
                    
                    _logger.LogWarning("Account service rejected Notification permission registration: {Message}", response.Message);
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    var delay = TimeSpan.FromSeconds(attempt * 2);
                    _logger.LogWarning(ex, "Permission registration attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds}s...", 
                        attempt, maxAttempts, delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Permission registration canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering Notification permissions via gRPC.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static RegisterPermissionsRequest BuildPermissionRegistrationRequest()
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
            new PermissionDefinitionDto { Name = NotificationPermissions.AuditLogs.Read, DisplayName = NotificationPermissions.AuditLogs.L.Read, GroupName = NotificationPermissions.GroupName }
        ]);

        return request;
    }
}
