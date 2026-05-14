using LHA.Auditing;
using LHA.AuditLog.Domain;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;
using LHA.Core.Users;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Notification.Infrastructure.Persistences;

/// <summary>
/// MongoDB-backed DbContext for the Notification Service, combining all notification module
/// entity mappings into a single context. Uses MongoDB.EntityFrameworkCore as the EF Core
/// provider, so all standard EF Core model-building APIs map to MongoDB collections.
/// </summary>
public sealed class NotificationDbContext
    : LhaDbContext<NotificationDbContext>, IHasEventOutbox, IHasEventInbox
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    internal DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

    internal DbSet<TemplateEntity> Templates => Set<TemplateEntity>();

    internal DbSet<UserPreferenceEntity> UserPreferences => Set<UserPreferenceEntity>();

    internal DbSet<TenantChannelConfigurationEntity> TenantChannelConfigurations => Set<TenantChannelConfigurationEntity>();

    internal DbSet<DeviceEntity> Devices => Set<DeviceEntity>();

    internal DbSet<NotificationBatchEntity> NotificationBatches => Set<NotificationBatchEntity>();

    internal DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();

    internal DbSet<AuditLogActionEntity> AuditLogActions => Set<AuditLogActionEntity>();

    internal DbSet<EntityChangeEntity> EntityChanges => Set<EntityChangeEntity>();

    internal DbSet<EntityPropertyChangeEntity> EntityPropertyChanges => Set<EntityPropertyChangeEntity>();

    internal DbSet<AuditLogPipelineEntity> AuditLogPipeline => Set<AuditLogPipelineEntity>();

    private readonly IServiceProvider _serviceProvider;

    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options,
        IServiceProvider serviceProvider,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null,
        ICurrentUser? currentUser = null)
        : base(options, auditPropertySetter, currentTenant, currentUser)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);

        var auditOptions = _serviceProvider?.GetService<Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>>();
        var auditMode = auditOptions?.Value.Mode ?? AuditLogStoreMode.All;
        modelBuilder.ConfigureAuditLogMongoDb(auditMode);
        modelBuilder.TryConfigureEventBus<NotificationDbContext>();

        // Notification module
        modelBuilder.Entity<NotificationEntity>().ToTable(DbSchemeConsts.Notification.Notifications);
        modelBuilder.Entity<TemplateEntity>().ToTable(DbSchemeConsts.Notification.Templates);
        modelBuilder.Entity<UserPreferenceEntity>().ToTable(DbSchemeConsts.Notification.UserPreferences);
        modelBuilder.Entity<TenantChannelConfigurationEntity>().ToTable(DbSchemeConsts.Notification.TenantChannelConfigs);
        modelBuilder.Entity<DeviceEntity>().ToTable(DbSchemeConsts.Notification.Devices);
        modelBuilder.Entity<NotificationBatchEntity>().ToTable(DbSchemeConsts.Notification.NotificationBatches);

        // Audit Log
        modelBuilder.Entity<AuditLogEntity>().ToTable(DbSchemeConsts.Audit.Log);
        modelBuilder.Entity<AuditLogActionEntity>().ToTable(DbSchemeConsts.Audit.Action);
        // modelBuilder.Entity<AuditLogPipelineEntity>().ToTable(DbSchemeConsts.Audit.LogPipeline);
        modelBuilder.Entity<EntityChangeEntity>().ToTable(DbSchemeConsts.Audit.EntityChange);
        modelBuilder.Entity<EntityPropertyChangeEntity>().ToTable(DbSchemeConsts.Audit.PropertyChange);

        // Event Bus
        modelBuilder.Entity<OutboxMessage>().ToTable(DbSchemeConsts.Event.Outbox);
        modelBuilder.Entity<InboxMessage>().ToTable(DbSchemeConsts.Event.Inbox);

        // ─── Global query filters (soft-delete, multi-tenant) ──────
        // Must be called AFTER all entities are configured.
        ApplyGlobalFilters(modelBuilder);
    }
}