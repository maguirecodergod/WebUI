using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.NotificationSentEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationSentEto : IntegrationEvent
    {
        public Guid NotificationId { get; private set; }
        public Guid RecipientId { get; private set; }

        public NotificationSentEto(Guid notificationId, Guid recipientId)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
        }
    }
}