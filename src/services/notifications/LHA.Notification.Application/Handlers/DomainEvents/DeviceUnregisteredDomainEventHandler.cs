using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class DeviceUnregisteredDomainEventHandler : IEventHandler<DeviceUnregisteredDomainEvent>
    {
        private readonly ILogger<DeviceUnregisteredDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public DeviceUnregisteredDomainEventHandler(
            ILogger<DeviceUnregisteredDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(DeviceUnregisteredDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing DeviceUnregisteredDomainEvent for device. DeviceId = {@event.DeviceId}, EventId = {context.Metadata.EventId}");

            var eventEto = new DeviceUnregisteredEto(@event.DeviceId, @event.UserId)
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishDeviceUnregisteredAsync(eventEto);

            _logger.LogInformation($"Published DeviceUnregisteredEto for device. DeviceId = {@event.DeviceId}, EventId = {context.Metadata.EventId}");
        }
    }
}
