using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Marker interface for a <see cref="DbContext"/> that supports the transactional outbox pattern.
/// <para>
/// Implement this on your DbContext and expose a <see cref="DbSet{TEntity}"/> of
/// <see cref="OutboxMessage"/>. The framework will auto-configure the table and register
/// <see cref="LHA.EventBus.IOutboxStore"/> when this interface is detected.
/// </para>
/// </summary>
/// <example>
/// <code>
/// public class AppDbContext : LhaDbContext&lt;AppDbContext&gt;, IHasEventOutbox
/// {
///     public DbSet&lt;OutboxMessage&gt; OutboxMessages =&gt; Set&lt;OutboxMessage&gt;();
/// }
/// </code>
/// </example>
public interface IHasEventOutbox
{
    /// <summary>Gets the outbox message set.</summary>
    DbSet<OutboxMessage> OutboxMessages { get; }
}
