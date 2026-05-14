using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class NotificationDeliveredDomainEventHandler : IEventHandler<NotificationDeliveredDomainEvent>
    {
        private readonly INotificationEventPublisher _notificationEventPublisher;
        private readonly ILogger<NotificationDeliveredDomainEventHandler> _logger;

        public NotificationDeliveredDomainEventHandler(
            INotificationEventPublisher notificationEventPublisher,
            ILogger<NotificationDeliveredDomainEventHandler> logger
        )
        {
            _logger = logger;
            _notificationEventPublisher = notificationEventPublisher;
        }

        public async Task HandleAsync(NotificationDeliveredDomainEvent @event,
            EventContext context,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationDeliveredDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationDeliveredEto(@event.NotificationId, @event.RecipientId)
            {
                TenantId = @event.TenantId,
            };

            await _notificationEventPublisher.PublishNotificationDeliveredAsync(eventEto);

            _logger.LogInformation($"Published NotificationDeliveredEto for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}