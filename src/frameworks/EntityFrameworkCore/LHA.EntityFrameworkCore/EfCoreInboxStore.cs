using LHA.EventBus;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IInboxStore"/> that works with any
/// <see cref="DbContext"/> implementing <see cref="IHasEventInbox"/>.
/// </summary>
/// <typeparam name="TDbContext">A DbContext type that implements <see cref="IHasEventInbox"/>.</typeparam>
public sealed class EfCoreInboxStore<TDbContext>(IDbContextProvider<TDbContext> dbContextProvider)
    : IInboxStore
    where TDbContext : DbContext, IHasEventInbox
{
    private async Task<IHasEventInbox> GetInboxContextAsync()
    {
        var dbContext = await dbContextProvider.GetDbContextAsync();
        return (IHasEventInbox)dbContext;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        Guid eventId,
        string consumerGroup,
        CancellationToken cancellationToken = default)
    {
        var inbox = await GetInboxContextAsync();

        return await inbox.InboxMessages
            .AsNoTracking()
            .AnyAsync(
                m => m.EventId == eventId && m.ConsumerGroup == consumerGroup,
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task SaveAsync(InboxMessageInfo message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var inbox = await GetInboxContextAsync();
        var entity = InboxMessage.FromInfo(message);

        // Add to tracker — SaveChanges is called by the Unit of Work.
        await inbox.InboxMessages.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkProcessedAsync(
        Guid eventId,
        string consumerGroup,
        CancellationToken cancellationToken = default)
    {
        var inbox = await GetInboxContextAsync();
        var now = TimeProvider.System.GetUtcNow();

        await inbox.InboxMessages
            .Where(m => m.EventId == eventId && m.ConsumerGroup == consumerGroup)
            .ExecuteUpdateAsync(
                s => s.SetProperty(m => m.ProcessedAtUtc, now),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task PurgeAsync(TimeSpan retentionPeriod, CancellationToken cancellationToken = default)
    {
        var inbox = await GetInboxContextAsync();
        var cutoff = TimeProvider.System.GetUtcNow() - retentionPeriod;

        await inbox.InboxMessages
            .Where(m => m.ProcessedAtUtc != null && m.ProcessedAtUtc < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
