using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Marker interface for a <see cref="DbContext"/> that supports the idempotent inbox pattern.
/// <para>
/// Implement this on your DbContext and expose a <see cref="DbSet{TEntity}"/> of
/// <see cref="InboxMessage"/>. The framework will auto-configure the table and register
/// <see cref="LHA.EventBus.IInboxStore"/> when this interface is detected.
/// </para>
/// </summary>
/// <example>
/// <code>
/// public class AppDbContext : LhaDbContext&lt;AppDbContext&gt;, IHasEventInbox
/// {
///     public DbSet&lt;InboxMessage&gt; InboxMessages =&gt; Set&lt;InboxMessage&gt;();
/// }
/// </code>
/// </example>
public interface IHasEventInbox
{
    /// <summary>Gets the inbox message set.</summary>
    DbSet<InboxMessage> InboxMessages { get; }
}
