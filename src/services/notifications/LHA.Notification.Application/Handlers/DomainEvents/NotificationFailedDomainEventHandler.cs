using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class NotificationFailedDomainEventHandler : IEventHandler<NotificationFailedDomainEvent>
    {
        private readonly ILogger<NotificationFailedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;


        public NotificationFailedDomainEventHandler(
            ILogger<NotificationFailedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher
        )
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(NotificationFailedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationFailedDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationFailedEto(@event.NotificationId, @event.RecipientId)
            {
                TenantId = @event.TenantId,
            };

            await _notificationPublisher.PublishNotificationFailedAsync(eventEto);

            _logger.LogInformation($"Processed NotificationFailedDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}