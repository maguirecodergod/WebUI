namespace LHA.Notification.Application.Contracts;

public interface INotificationEventPublisher
{
    Task PublishNotificationCreatedAsync(NotificationCreatedEto eventEto);
    Task PublishNotificationSentAsync(NotificationSentEto eventEto);
    Task PublishNotificationDeliveredAsync(NotificationDeliveredEto eventEto);
    Task PublishNotificationReadAsync(NotificationReadEto eventEto);
    Task PublishNotificationFailedAsync(NotificationFailedEto eventEto);
    Task PublishNotificationExpiredAsync(NotificationExpiredEto eventEto);
    Task PublishDeviceRegisteredAsync(DeviceRegisteredEto eventEto);
    Task PublishDeviceUnregisteredAsync(DeviceUnregisteredEto eventEto);
    Task PublishTemplateCreatedAsync(TemplateCreatedEto eventEto);
    Task PublishTemplateUpdatedAsync(TemplateUpdatedEto eventEto);
    Task PublishBatchCreatedAsync(BatchCreatedEto eventEto);
    Task PublishBatchCompletedAsync(BatchCompletedEto eventEto);
}
