using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class TemplateUpdatedDomainEventHandler : IEventHandler<TemplateUpdatedDomainEvent>
    {
        private readonly ILogger<TemplateUpdatedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public TemplateUpdatedDomainEventHandler(
            ILogger<TemplateUpdatedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(TemplateUpdatedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing TemplateUpdatedDomainEvent for template. TemplateId = {@event.TemplateId}, EventId = {context.Metadata.EventId}");

            var eventEto = new TemplateUpdatedEto(@event.TemplateId, @event.Code)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishTemplateUpdatedAsync(eventEto);

            _logger.LogInformation($"Published TemplateUpdatedEto for template. TemplateId = {@event.TemplateId}, EventId = {context.Metadata.EventId}");
        }
    }
}
