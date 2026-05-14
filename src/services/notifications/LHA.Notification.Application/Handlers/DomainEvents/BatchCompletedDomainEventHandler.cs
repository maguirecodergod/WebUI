using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class BatchCompletedDomainEventHandler : IEventHandler<BatchCompletedDomainEvent>
    {
        private readonly ILogger<BatchCompletedDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public BatchCompletedDomainEventHandler(
            ILogger<BatchCompletedDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(BatchCompletedDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing BatchCompletedDomainEvent for batch. BatchId = {@event.BatchId}, EventId = {context.Metadata.EventId}");

            var eventEto = new BatchCompletedEto(@event.BatchId, @event.Name, @event.TotalCount, @event.DeliveredCount, @event.FailedCount)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishBatchCompletedAsync(eventEto);

            _logger.LogInformation($"Published BatchCompletedEto for batch. BatchId = {@event.BatchId}, EventId = {context.Metadata.EventId}");
        }
    }
}
