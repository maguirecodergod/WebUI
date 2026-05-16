namespace LHA.Notification.Domain.Shared;

public static class QueueConstants
{
    public const string NotificationSendQueue = "notification.send";
    public const string NotificationSendBulkQueue = "notification.send.bulk";
    public const string NotificationSendCriticalQueue = "notification.send.critical";
    public const string NotificationRetryQueue = "notification.retry";
    public const string NotificationBatchChunkQueue = "notification.batch.chunk";
    public const string NotificationScheduledQueue = "notification.scheduled";
    public const string NotificationDeliveryReceiptQueue = "notification.delivery.receipt";
    public const string VirtualHost = "/notifications";
    public const int NotificationSendPrefetchCount = 50;
    public const int NotificationSendBulkPrefetchCount = 10;
    public const int NotificationCriticalPrefetchCount = 100;
    public const int NotificationBatchChunkPrefetchCount = 20;
    public const int NotificationRetryMaxRetries = 3;
}
