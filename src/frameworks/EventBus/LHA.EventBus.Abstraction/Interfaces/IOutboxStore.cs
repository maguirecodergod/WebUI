namespace LHA.EventBus
{
    /// <summary>
    /// Persistence abstraction for the transactional outbox.
    /// Implementations should be scoped to the same database/transaction as business data.
    /// </summary>
    public interface IOutboxStore
    {
        /// <summary>Atomically saves an outbox message (within the current transaction).</summary>
        Task SaveAsync(OutboxMessageInfo message, CancellationToken cancellationToken = default);

        /// <summary>Returns the next batch of unprocessed messages ordered by creation time.</summary>
        Task<IReadOnlyList<OutboxMessageInfo>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default);

        /// <summary>Marks a message as successfully forwarded to the transport.</summary>
        Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>Increments the retry count for a failed message.</summary>
        Task IncrementRetryAsync(Guid id, CancellationToken cancellationToken = default);
    }
}