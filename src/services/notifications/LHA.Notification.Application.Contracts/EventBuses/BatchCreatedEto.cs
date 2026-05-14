using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.BatchCreatedEto")]
    [EventVersion(version: 1)]
    public sealed record BatchCreatedEto : IntegrationEvent
    {
        public Guid BatchId { get; private set; }
        public string Name { get; private set; }
        public long TotalCount { get; private set; }

        public BatchCreatedEto(Guid batchId, string name, long totalCount)
        {
            BatchId = batchId;
            Name = name;
            TotalCount = totalCount;
        }
    }
}
