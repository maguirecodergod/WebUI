using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class NotificationSentDomainEventHandler : IEventHandler<NotificationSentDomainEvent>
    {
        private readonly ILogger<NotificationSentDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public NotificationSentDomainEventHandler(
            ILogger<NotificationSentDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher
        )
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(NotificationSentDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationSentDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationSentEto(@event.NotificationId, @event.RecipientId)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishNotificationSentAsync(eventEto);

            _logger.LogInformation($"Published NotificationSentEto for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}
