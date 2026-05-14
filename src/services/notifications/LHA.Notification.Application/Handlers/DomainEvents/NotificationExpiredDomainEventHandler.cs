using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class NotificationExpiredDomainEventHandler : IEventHandler<NotificationExpiredDomainEvent>
    {
        private readonly ILogger<NotificationExpiredDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public NotificationExpiredDomainEventHandler(
            ILogger<NotificationExpiredDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(NotificationExpiredDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationExpiredDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationExpiredEto(@event.NotificationId, @event.RecipientId)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishNotificationExpiredAsync(eventEto);

            _logger.LogInformation($"Published NotificationExpiredEto for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}
