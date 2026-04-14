using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.Mega.Domain.Account;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace LHA.Mega.EntityFrameworkCore;

public sealed class MegaDbContext
    : LhaDbContext<MegaDbContext>, IHasEventOutbox, IHasEventInbox
{
    public DbSet<MegaAccountEntity> Accounts => Set<MegaAccountEntity>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public MegaDbContext(DbContextOptions<MegaDbContext> options) : base(options) { }

    public MegaDbContext(
        DbContextOptions<MegaDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureMega();
        modelBuilder.TryConfigureEventBus<MegaDbContext>();
        
        // Apply global query filters (IMultiTenant, ISoftDelete) after entities are added
        ApplyGlobalFilters(modelBuilder);
    }
}
