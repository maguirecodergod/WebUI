using LHA.EventBus;
using LHA.Notification.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure
{
    public class NotificationEventPublisher : INotificationEventPublisher
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<NotificationEventPublisher> _logger;

        public NotificationEventPublisher(
            IEventBus eventBus,
            ILogger<NotificationEventPublisher> logger
        )
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public async Task PublishBatchCompletedAsync(BatchCompletedEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishBatchCreatedAsync(BatchCreatedEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishDeviceRegisteredAsync(DeviceRegisteredEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishDeviceUnregisteredAsync(DeviceUnregisteredEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationCreatedAsync(NotificationCreatedEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationDeliveredAsync(NotificationDeliveredEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationExpiredAsync(NotificationExpiredEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationFailedAsync(NotificationFailedEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationReadAsync(NotificationReadEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishNotificationSentAsync(NotificationSentEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishTemplateCreatedAsync(TemplateCreatedEto eventEto)
        {
            throw new NotImplementedException();
        }

        public async Task PublishTemplateUpdatedAsync(TemplateUpdatedEto eventEto)
        {
            throw new NotImplementedException();
        }
    }
}