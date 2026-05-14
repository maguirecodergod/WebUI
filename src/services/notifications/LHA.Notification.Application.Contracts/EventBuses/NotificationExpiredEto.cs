using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.NotificationExpiredEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationExpiredEto : IntegrationEvent
    {
        public Guid NotificationId { get; private set; }
        public Guid RecipientId { get; private set; }

        public NotificationExpiredEto(Guid notificationId, Guid recipientId)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
        }
    }
}
