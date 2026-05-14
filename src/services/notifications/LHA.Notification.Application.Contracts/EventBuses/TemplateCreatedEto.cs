using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.TemplateCreatedEto")]
    [EventVersion(version: 1)]
    public sealed record TemplateCreatedEto : IntegrationEvent
    {
        public Guid TemplateId { get; private set; }
        public string Code { get; private set; }

        public TemplateCreatedEto(Guid templateId, string code)
        {
            TemplateId = templateId;
            Code = code;
        }
    }
}
