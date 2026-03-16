namespace LHA.EventBus;
/// <summary>
/// Null-object inbox store. Does not track messages — every event is considered new.
/// Used as the default before a real inbox store is registered.
/// </summary>
internal sealed class NullInboxStore : IInboxStore
{
    public Task<bool> ExistsAsync(Guid eventId, string consumerGroup, CancellationToken cancellationToken = default)
        => Task.FromResult(false);

    public Task SaveAsync(InboxMessageInfo message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task MarkProcessedAsync(Guid eventId, string consumerGroup, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task PurgeAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
