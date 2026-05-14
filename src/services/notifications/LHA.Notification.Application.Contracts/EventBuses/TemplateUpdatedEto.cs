using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.TemplateUpdatedEto")]
    [EventVersion(version: 1)]
    public sealed record TemplateUpdatedEto : IntegrationEvent
    {
        public Guid TemplateId { get; private set; }
        public string Code { get; private set; }

        public TemplateUpdatedEto(Guid templateId, string code)
        {
            TemplateId = templateId;
            Code = code;
        }
    }
}
