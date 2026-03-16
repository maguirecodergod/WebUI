namespace LHA.EventBus
{
    /// <summary>
    /// Null-object outbox store that discards messages.
    /// Used as the default before a real outbox store is registered.
    /// </summary>
    internal sealed class NullOutboxStore : IOutboxStore
    {
        public Task SaveAsync(OutboxMessageInfo message, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IReadOnlyList<OutboxMessageInfo>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboxMessageInfo>>([]);
        public Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task IncrementRetryAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}