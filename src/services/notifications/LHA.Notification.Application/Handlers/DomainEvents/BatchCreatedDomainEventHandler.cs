using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class BatchCreatedDomainEventHandler : IEventHandler<BatchCreatedDomainEvent>
    {
        private readonly ILogger<BatchCreatedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public BatchCreatedDomainEventHandler(
            ILogger<BatchCreatedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(BatchCreatedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing BatchCreatedDomainEvent for batch. BatchId = {@event.BatchId}, EventId = {context.Metadata.EventId}");

            var eventEto = new BatchCreatedEto(@event.BatchId, @event.Name, @event.TotalCount)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishBatchCreatedAsync(eventEto);

            _logger.LogInformation($"Published BatchCreatedEto for batch. BatchId = {@event.BatchId}, EventId = {context.Metadata.EventId}");
        }
    }
}
