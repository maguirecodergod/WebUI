using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.PermissionManagement.EntityFrameworkCore;

public sealed class PermissionManagementDbContext
    : LhaDbContext<PermissionManagementDbContext>, IHasEventOutbox, IHasEventInbox
{
    public DbSet<PermissionDefinition> PermissionDefinitions => Set<PermissionDefinition>();
    public DbSet<PermissionGroup> PermissionGroups => Set<PermissionGroup>();
    public DbSet<PermissionGroupItem> PermissionGroupItems => Set<PermissionGroupItem>();
    public DbSet<PermissionTemplate> PermissionTemplates => Set<PermissionTemplate>();
    public DbSet<PermissionTemplateItem> PermissionTemplateItems => Set<PermissionTemplateItem>();
    public DbSet<PermissionGrant> PermissionGrants => Set<PermissionGrant>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    public PermissionManagementDbContext(
        DbContextOptions<PermissionManagementDbContext> options)
        : base(options)
    {
    }

    public PermissionManagementDbContext(
        DbContextOptions<PermissionManagementDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.TryConfigureEventBus<PermissionManagementDbContext>();
    }
}
