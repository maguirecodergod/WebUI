namespace LHA.EventBus
{
    /// <summary>
    /// Persistence abstraction for the idempotent inbox.
    /// Enables exactly-once processing semantics over at-least-once transports.
    /// </summary>
    public interface IInboxStore
    {
        /// <summary>
        /// Returns <c>true</c> if the event has already been processed by the given consumer group.
        /// </summary>
        Task<bool> ExistsAsync(Guid eventId, string consumerGroup, CancellationToken cancellationToken = default);

        /// <summary>Records an incoming event in the inbox.</summary>
        Task SaveAsync(InboxMessageInfo message, CancellationToken cancellationToken = default);

        /// <summary>Marks an event as successfully processed.</summary>
        Task MarkProcessedAsync(Guid eventId, string consumerGroup, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cleans up old inbox entries beyond the retention period.
        /// </summary>
        Task PurgeAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default);
    }
}