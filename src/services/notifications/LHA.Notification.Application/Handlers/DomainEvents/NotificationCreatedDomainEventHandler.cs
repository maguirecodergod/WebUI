using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application
{
    public class NotificationCreatedDomainEventHandler : IEventHandler<NotificationCreatedDomainEvent>
    {
        private readonly ILogger<NotificationCreatedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public NotificationCreatedDomainEventHandler(
            ILogger<NotificationCreatedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(NotificationCreatedDomainEvent @event,
            EventContext context,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing NotificationCreatedDomainEvent for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");

            var eventEto = new NotificationCreatedEto()
            {
                NotificationId = @event.NotificationId,
                TenantId = @event.TenantId,
                RecipientId = @event.RecipientId,
                Type = @event.Type,
                Priority = @event.Priority,
                Subject = @event.Subject,
                Body = @event.Body,
                TemplateId = @event.TemplateId,
                TemplateVariables = @event.TemplateVariables,
                ActionUrl = @event.ActionUrl,
                Data = @event.Data
            };


            await _notificationPublisher.PublishNotificationCreatedAsync(eventEto);

            _logger.LogInformation($"Published NotificationCreatedEto for notification. NotificationId = {@event.NotificationId}, EventId = {context.Metadata.EventId}");
        }
    }
}