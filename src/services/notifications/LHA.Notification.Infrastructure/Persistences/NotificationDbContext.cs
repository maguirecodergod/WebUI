using LHA.Auditing;
using LHA.AuditLog.Domain;
using LHA.AuditLog.Domain.Shared;
using LHA.AuditLog.EntityFrameworkCore.Contracts.Options;
using LHA.AuditLog.EntityFrameworkCore.MongoDB;
using LHA.Core.Users;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Notification.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace LHA.Notification.Infrastructure.Persistences;

/// <summary>
/// MongoDB-backed DbContext for the Notification Service.
/// This context uses composition by calling module configuration extensions 
/// (e.g., ConfigureAuditLogMongoDb) instead of inheritance, allowing for 
/// a more modular and flexible architecture.
/// </summary>
public sealed class NotificationDbContext
    : LhaDbContext<NotificationDbContext>, IHasEventOutbox, IHasEventInbox
{
    // --- Framework Entities ---
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    // --- Notification Entities ---
    public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();
    public DbSet<TemplateEntity> Templates => Set<TemplateEntity>();
    public DbSet<UserPreferenceEntity> UserPreferences => Set<UserPreferenceEntity>();
    public DbSet<ChannelConfigurationEntity> TenantChannelConfigurations => Set<ChannelConfigurationEntity>();
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<NotificationBatchEntity> NotificationBatches => Set<NotificationBatchEntity>();

    private readonly Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? _auditOptions;

    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options,
        Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? auditOptions = null,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null,
        ICurrentUser? currentUser = null)
        : base(options, auditPropertySetter, currentTenant, currentUser)
    {
        _auditOptions = auditOptions;
        
        // Standalone MongoDB instances do not support transactions.
        // Disable auto-transaction behavior to allow SaveChanges to work.
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Module Configurations (Composition) ---
        
        // 1. Audit Log Module
        var auditMode = _auditOptions?.Value.Mode ?? CAuditLogStoreMode.All;
        modelBuilder.ConfigureAuditLogMongoDb(auditMode);

        // 2. Notification Module
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
        
        modelBuilder.Entity<NotificationEntity>().ToCollection(DbSchemeConsts.Notification.Notifications);
        modelBuilder.Entity<TemplateEntity>().ToCollection(DbSchemeConsts.Notification.Templates);
        modelBuilder.Entity<UserPreferenceEntity>().ToCollection(DbSchemeConsts.Notification.UserPreferences);
        modelBuilder.Entity<ChannelConfigurationEntity>().ToCollection(DbSchemeConsts.Notification.TenantChannelConfigs);
        modelBuilder.Entity<DeviceEntity>().ToCollection(DbSchemeConsts.Notification.Devices);
        modelBuilder.Entity<NotificationBatchEntity>().ToCollection(DbSchemeConsts.Notification.NotificationBatches);
        
        // --- Audit Log Collection Mapping (Ensuring consistency) ---
        modelBuilder.Entity<AuditLogEntity>().ToCollection(DbSchemeConsts.Audit.Log);
        modelBuilder.Entity<AuditLogPipelineEntity>().ToCollection(DbSchemeConsts.Audit.LogPipeline);

        // 3. Event Bus / Outbox
        modelBuilder.Entity<OutboxMessage>().ToCollection(DbSchemeConsts.Event.Outbox);
        modelBuilder.Entity<InboxMessage>().ToCollection(DbSchemeConsts.Event.Inbox);

        // Apply global query filters (IMultiTenant, ISoftDelete) after all entities are added
        ApplyGlobalFilters(modelBuilder);
    }
}
