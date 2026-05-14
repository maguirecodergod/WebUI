using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.NotificationFailedEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationFailedEto : IntegrationEvent
    {
        public Guid NotificationId { get; private set; }
        public Guid RecipientId { get; private set; }

        public NotificationFailedEto(Guid notificationId, Guid recipientId)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
        }
    }
}