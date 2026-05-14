using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName("LHA.Notification.Application.Contracts.NotificationDeliveredEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationDeliveredEto : IntegrationEvent
    {
        public Guid NotificationId { get; private set; }
        public Guid RecipientId { get; private set; }

        public NotificationDeliveredEto(Guid notificationId, Guid recipientId)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
        }
    }
}