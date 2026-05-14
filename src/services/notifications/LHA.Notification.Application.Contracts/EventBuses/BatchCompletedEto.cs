using LHA.EventBus;

namespace LHA.Notification.Application.Contracts
{
    [EventName(name: "LHA.Notification.Application.Contracts.BatchCompletedEto")]
    [EventVersion(version: 1)]
    public sealed record BatchCompletedEto : IntegrationEvent
    {
        public Guid BatchId { get; private set; }
        public string Name { get; private set; }
        public long TotalCount { get; private set; }
        public long DeliveredCount { get; private set; }
        public long FailedCount { get; private set; }

        public BatchCompletedEto(Guid batchId, string name, long totalCount, long deliveredCount, long failedCount)
        {
            BatchId = batchId;
            Name = name;
            TotalCount = totalCount;
            DeliveredCount = deliveredCount;
            FailedCount = failedCount;
        }
    }
}
