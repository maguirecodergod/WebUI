using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class TemplateCreatedDomainEventHandler : IEventHandler<TemplateCreatedDomainEvent>
    {
        private readonly ILogger<TemplateCreatedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public TemplateCreatedDomainEventHandler(
            ILogger<TemplateCreatedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(TemplateCreatedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing TemplateCreatedDomainEvent for template. TemplateId = {@event.TemplateId}, EventId = {context.Metadata.EventId}");

            var eventEto = new TemplateCreatedEto(@event.TemplateId, @event.Code)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishTemplateCreatedAsync(eventEto);

            _logger.LogInformation($"Published TemplateCreatedEto for template. TemplateId = {@event.TemplateId}, EventId = {context.Metadata.EventId}");
        }
    }
}
