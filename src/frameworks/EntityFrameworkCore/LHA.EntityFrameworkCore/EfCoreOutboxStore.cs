using LHA.EventBus;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// EF Core implementation of <see cref="IOutboxStore"/> that works with any
/// <see cref="DbContext"/> implementing <see cref="IHasEventOutbox"/>.
/// <para>
/// <see cref="SaveAsync"/> adds the entity to the change tracker without calling SaveChanges —
/// the Unit of Work will flush all changes atomically. Read and update operations use
/// <c>AsNoTracking</c> / <c>ExecuteUpdateAsync</c> for efficiency.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">A DbContext type that implements <see cref="IHasEventOutbox"/>.</typeparam>
public sealed class EfCoreOutboxStore<TDbContext>(IDbContextProvider<TDbContext> dbContextProvider)
    : IOutboxStore
    where TDbContext : DbContext, IHasEventOutbox
{
    private async Task<IHasEventOutbox> GetOutboxContextAsync()
    {
        var dbContext = await dbContextProvider.GetDbContextAsync();
        return (IHasEventOutbox)dbContext;
    }

    /// <inheritdoc />
    public async Task SaveAsync(OutboxMessageInfo message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var outbox = await GetOutboxContextAsync();
        var entity = OutboxMessage.FromInfo(message);

        // Add to tracker — SaveChanges is called by the Unit of Work.
        await outbox.OutboxMessages.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<OutboxMessageInfo>> GetPendingAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var outbox = await GetOutboxContextAsync();

        var entities = await outbox.OutboxMessages
            .AsNoTracking()
            .Where(m => m.ProcessedAtUtc == null)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        return entities.ConvertAll(e => e.ToInfo());
    }

    /// <inheritdoc />
    public async Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var outbox = await GetOutboxContextAsync();
        var now = TimeProvider.System.GetUtcNow();

        await outbox.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(m => m.ProcessedAtUtc, now),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task IncrementRetryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var outbox = await GetOutboxContextAsync();

        await outbox.OutboxMessages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(
                s => s.SetProperty(m => m.RetryCount, m => m.RetryCount + 1),
                cancellationToken);
    }
}
