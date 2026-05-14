using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.DeviceUnregisteredEto")]
    [EventVersion(version: 1)]
    public sealed record DeviceUnregisteredEto : IntegrationEvent
    {
        public Guid DeviceId { get; private set; }
        public Guid UserId { get; private set; }

        public DeviceUnregisteredEto(Guid deviceId, Guid userId)
        {
            DeviceId = deviceId;
            UserId = userId;
        }
    }
}
