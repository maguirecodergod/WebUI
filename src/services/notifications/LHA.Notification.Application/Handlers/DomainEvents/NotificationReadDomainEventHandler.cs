using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class NotificationReadDomainEventHandler : IEventHandler<NotificationReadDomainEvent>
    {
        private readonly ILogger<NotificationReadDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public NotificationReadDomainEventHandler(
            ILogger<NotificationReadDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(NotificationReadDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationReadDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationReadEto(@event.NotificationId, @event.RecipientId)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishNotificationReadAsync(eventEto);

            _logger.LogInformation($"Published NotificationReadEto for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}
