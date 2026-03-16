using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core DbContext for the Identity module.
/// <para>
/// Implements <see cref="IHasEventOutbox"/> and <see cref="IHasEventInbox"/> for
/// transactional outbox/inbox domain event delivery.
/// </para>
/// </summary>
public sealed class IdentityDbContext
    : LhaDbContext<IdentityDbContext>, IHasEventOutbox, IHasEventInbox
{
    // ─── Aggregate roots ─────────────────────────────────────────────
    public DbSet<IdentityUser> Users => Set<IdentityUser>();
    public DbSet<IdentityRole> Roles => Set<IdentityRole>();
    public DbSet<IdentityClaimType> ClaimTypes => Set<IdentityClaimType>();

    // ─── Sub-entities ────────────────────────────────────────────────
    public DbSet<IdentityUserRole> UserRoles => Set<IdentityUserRole>();
    public DbSet<IdentityUserClaim> UserClaims => Set<IdentityUserClaim>();
    public DbSet<IdentityUserLogin> UserLogins => Set<IdentityUserLogin>();
    public DbSet<IdentityUserToken> UserTokens => Set<IdentityUserToken>();
    public DbSet<IdentityRoleClaim> RoleClaims => Set<IdentityRoleClaim>();

    // ─── Standalone entities ─────────────────────────────────────────
    public DbSet<IdentitySecurityLog> SecurityLogs => Set<IdentitySecurityLog>();
    public DbSet<IdentityPermissionGrant> PermissionGrants => Set<IdentityPermissionGrant>();

    // ─── Outbox / Inbox ──────────────────────────────────────────────
    /// <inheritdoc />
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    /// <inheritdoc />
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureIdentity();
        modelBuilder.TryConfigureEventBus<IdentityDbContext>();
    }
}
