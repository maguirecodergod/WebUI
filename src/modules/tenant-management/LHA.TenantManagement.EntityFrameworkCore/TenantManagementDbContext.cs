using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.TenantManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.TenantManagement.EntityFrameworkCore;

/// <summary>
/// EF Core DbContext for the Tenant Management module.
/// <para>
/// Implements <see cref="IHasEventOutbox"/> and <see cref="IHasEventInbox"/> to opt into
/// the transactional outbox/inbox pattern for domain event delivery.
/// </para>
/// </summary>
public sealed class TenantManagementDbContext
    : LhaDbContext<TenantManagementDbContext>, IHasEventOutbox, IHasEventInbox
{
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();
    public DbSet<TenantConnectionString> TenantConnectionStrings => Set<TenantConnectionString>();

    /// <inheritdoc />
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public TenantManagementDbContext(DbContextOptions<TenantManagementDbContext> options)
        : base(options)
    {
    }

    public TenantManagementDbContext(
        DbContextOptions<TenantManagementDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureTenantManagement();
        modelBuilder.TryConfigureEventBus<TenantManagementDbContext>();
        
        // Apply global query filters (IMultiTenant, ISoftDelete) after entities are added
        base.OnModelCreating(modelBuilder);
    }
}
