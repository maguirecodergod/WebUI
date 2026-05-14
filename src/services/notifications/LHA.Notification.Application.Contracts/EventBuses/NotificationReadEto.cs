using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.NotificationReadEto")]
    [EventVersion(version: 1)]
    public sealed record NotificationReadEto : IntegrationEvent
    {
        public Guid NotificationId { get; private set; }
        public Guid RecipientId { get; private set; }

        public NotificationReadEto(Guid notificationId, Guid recipientId)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
        }
    }
}
