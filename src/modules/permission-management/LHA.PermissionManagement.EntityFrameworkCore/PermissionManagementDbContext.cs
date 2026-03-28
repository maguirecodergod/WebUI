using LHA.Auditing;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace LHA.PermissionManagement.EntityFrameworkCore;

public sealed class PermissionManagementDbContext
    : LhaDbContext<PermissionManagementDbContext>, IHasEventOutbox, IHasEventInbox
{
    public DbSet<PermissionDefinitionEntity> PermissionDefinitions => Set<PermissionDefinitionEntity>();
    public DbSet<PermissionGroupEntity> PermissionGroups => Set<PermissionGroupEntity>();
    public DbSet<PermissionGroupItemEntity> PermissionGroupItems => Set<PermissionGroupItemEntity>();
    public DbSet<PermissionTemplateEntity> PermissionTemplates => Set<PermissionTemplateEntity>();
    public DbSet<PermissionTemplateItemEntity> PermissionTemplateItems => Set<PermissionTemplateItemEntity>();
    public DbSet<PermissionGrantEntity> PermissionGrants => Set<PermissionGrantEntity>();

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
