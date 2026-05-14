using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.DeviceRegisteredEto")]
    [EventVersion(version: 1)]
    public sealed record DeviceRegisteredEto : IntegrationEvent
    {
        public Guid DeviceId { get; private set; }
        public Guid UserId { get; private set; }
        public string Platform { get; private set; }

        public DeviceRegisteredEto(Guid deviceId, Guid userId, string platform)
        {
            DeviceId = deviceId;
            UserId = userId;
            Platform = platform;
        }
    }
}
