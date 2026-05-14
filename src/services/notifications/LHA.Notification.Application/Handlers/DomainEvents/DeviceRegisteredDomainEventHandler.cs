using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Application.Handlers.DomainEvents
{
    public class DeviceRegisteredDomainEventHandler : IEventHandler<DeviceRegisteredDomainEvent>
    {
        private readonly ILogger<DeviceRegisteredDomainEventHandler> _logger;
        private readonly INotificationEventPublisher _notificationPublisher;

        public DeviceRegisteredDomainEventHandler(
            ILogger<DeviceRegisteredDomainEventHandler> logger,
            INotificationEventPublisher notificationPublisher)
        {
            _logger = logger;
            _notificationPublisher = notificationPublisher;
        }

        public async Task HandleAsync(DeviceRegisteredDomainEvent @event, EventContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Processing DeviceRegisteredDomainEvent for device. DeviceId = {@event.DeviceId}, EventId = {context.Metadata.EventId}");

            var eventEto = new DeviceRegisteredEto(@event.DeviceId, @event.UserId, @event.Platform.ToString())
            {
                TenantId = @event.TenantId
            };

            await _notificationPublisher.PublishDeviceRegisteredAsync(eventEto);

            _logger.LogInformation($"Published DeviceRegisteredEto for device. DeviceId = {@event.DeviceId}, EventId = {context.Metadata.EventId}");
        }
    }
}
