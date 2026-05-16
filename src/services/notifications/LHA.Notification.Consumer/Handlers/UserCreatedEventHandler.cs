using LHA.EventBus;
using LHA.Identity.Application.Contracts;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Consumer.Handlers;

public sealed class UserCreatedEventHandler(
    ILogger<UserCreatedEventHandler> logger,
    INotificationService notificationService,
    LHA.MultiTenancy.ICurrentTenant currentTenant)
    : IEventHandler<UserCreatedEto>
{
    public async Task HandleAsync(UserCreatedEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing UserCreatedEto for User ID: {UserId}, Tenant: {TenantId}", @event.UserId, @event.TenantId);

        var channels = new List<CNotificationChannel>
        {
            CNotificationChannel.Email,
            CNotificationChannel.Push
        };

        var notificationDto = new SendNotificationDto(
            RecipientId: @event.UserId,
            RecipientType: CRecipientType.User,
            Type: CNotificationType.System,
            Priority: CNotificationPriority.High,
            Subject: "Welcome to LHA System",
            Body: $"Hi {@event.UserName}, welcome to our platform! Please explore the features we have tailored for you.",
            Data: new Dictionary<string, string>
            {
                { "UserName", @event.UserName },
                { "Email", @event.Email }
            },
            ImageUrl: null,
            ActionUrl: "/profile",
            TemplateId: null,
            TemplateVariables: null,
            Tags: new List<string> { "Welcome", "Onboarding" },
            ExpiresAt: null,
            Channels: channels,
            SkipRateLimit: true
        );

        // We use the tenant ID from the event to ensure multi-tenancy context if needed,
        // although INotificationService handles tenant from ICurrentTenant. 
        using (currentTenant.Change(@event.TenantId))
        {
            await notificationService.SendAsync(notificationDto, cancellationToken);
        }
        
        logger.LogInformation("Successfully created welcome notification for User ID: {UserId}", @event.UserId);
    }
}
