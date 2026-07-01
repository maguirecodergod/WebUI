using LHA.Auditing;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.Identity.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.TenantManagement.EntityFrameworkCore;
using LHA.Core.Users;
using Microsoft.EntityFrameworkCore;
using LHA.AuditLog.EntityFrameworkCore.PostgreSQL;
using LHA.AuditLog.EntityFrameworkCore.Contracts.Options;
using LHA.AuditLog.Domain.Shared;

namespace LHA.Account.EntityFrameworkCore;

/// <summary>
/// Unified DbContext for the Account Service, combining all module entity mappings
/// into a single context. This context uses composition by calling module configuration 
/// extensions instead of inheritance, ensuring a modular and extensible architecture.
/// </summary>
public sealed class AccountDbContext
    : LhaDbContext<AccountDbContext>, IHasEventOutbox, IHasEventInbox
{
    /// <inheritdoc />
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    private readonly LHA.EntityFrameworkCore.Auditing.DataAuditingSaveChangesInterceptor? _auditInterceptor;
    private readonly Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? _auditOptions;

    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? auditOptions = null,
        LHA.EntityFrameworkCore.Auditing.DataAuditingSaveChangesInterceptor? auditInterceptor = null,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null,
        ICurrentUser? currentUser = null)
        : base(options, auditPropertySetter, currentTenant, currentUser)
    {
        _auditOptions = auditOptions;
        _auditInterceptor = auditInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (_auditInterceptor is not null)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Module Configurations (Composition) ---

        // 1. Identity Module
        modelBuilder.ConfigureIdentity();

        // 2. Tenant Management Module
        modelBuilder.ConfigureTenantManagement();

        // 3. Audit Log Module
        var auditMode = _auditOptions?.Value.Mode ?? CAuditLogStoreMode.All;
        modelBuilder.ConfigureAuditLogPostgreSql(auditMode); 

        // 4. Permission Management Module
        modelBuilder.ConfigurePermissionManagement();

        // 5. Event Bus
        modelBuilder.TryConfigureEventBus<AccountDbContext>();

        // ─── Override table names with Account Service conventions ───

        // Identity (Auth)
        modelBuilder.Entity<Identity.Domain.IdentityUser>().ToTable(DbSchemeConsts.Auth.User);
        modelBuilder.Entity<Identity.Domain.IdentityUserRole>().ToTable(DbSchemeConsts.Auth.UserRole);
        modelBuilder.Entity<Identity.Domain.IdentityUserClaim>().ToTable(DbSchemeConsts.Auth.UserClaim);
        modelBuilder.Entity<Identity.Domain.IdentityUserLogin>().ToTable(DbSchemeConsts.Auth.UserLogin);
        modelBuilder.Entity<Identity.Domain.IdentityUserToken>().ToTable(DbSchemeConsts.Auth.UserToken);
        modelBuilder.Entity<Identity.Domain.IdentityRole>().ToTable(DbSchemeConsts.Auth.Role);
        modelBuilder.Entity<Identity.Domain.IdentityRoleClaim>().ToTable(DbSchemeConsts.Auth.RoleClaim);
        modelBuilder.Entity<Identity.Domain.IdentityClaimType>().ToTable(DbSchemeConsts.Auth.ClaimType);
        modelBuilder.Entity<Identity.Domain.IdentitySecurityLog>().ToTable(DbSchemeConsts.Auth.SecurityLog);
        modelBuilder.Entity<Identity.Domain.IdentityPermissionGrant>().ToTable(DbSchemeConsts.Auth.PermissionGrant);
        modelBuilder.Entity<Identity.Domain.IdentityUserTenantIndex>().ToTable(DbSchemeConsts.Auth.UserTenantIndex);

        // Tenant Management
        modelBuilder.Entity<TenantManagement.Domain.TenantEntity>().ToTable(DbSchemeConsts.Tenant.Main);
        modelBuilder.Entity<TenantManagement.Domain.TenantConnectionString>().ToTable(DbSchemeConsts.Tenant.ConnectionString);

        // Audit Log
        modelBuilder.Entity<AuditLog.Domain.AuditLogEntity>().ToTable(DbSchemeConsts.Audit.Log);
        modelBuilder.Entity<AuditLog.Domain.AuditLogActionEntity>().ToTable(DbSchemeConsts.Audit.Action);
        modelBuilder.Entity<AuditLog.Domain.EntityChangeEntity>().ToTable(DbSchemeConsts.Audit.EntityChange);
        modelBuilder.Entity<AuditLog.Domain.EntityPropertyChangeEntity>().ToTable(DbSchemeConsts.Audit.PropertyChange);

        // Permission Management
        modelBuilder.Entity<PermissionManagement.Domain.PermissionDefinitions.PermissionDefinitionEntity>().ToTable(DbSchemeConsts.Permission.Definition);
        modelBuilder.Entity<PermissionManagement.Domain.PermissionGroups.PermissionGroupEntity>().ToTable(DbSchemeConsts.Permission.PermGroup);
        modelBuilder.Entity<PermissionManagement.Domain.PermissionGroups.PermissionGroupItemEntity>().ToTable(DbSchemeConsts.Permission.GroupItem);
        modelBuilder.Entity<PermissionManagement.Domain.PermissionTemplates.PermissionTemplateEntity>().ToTable(DbSchemeConsts.Permission.Template);
        modelBuilder.Entity<PermissionManagement.Domain.PermissionTemplates.PermissionTemplateItemEntity>().ToTable(DbSchemeConsts.Permission.TemplateItem);
        modelBuilder.Entity<PermissionManagement.Domain.PermissionGrants.PermissionGrantEntity>().ToTable(DbSchemeConsts.Permission.Grant);

        // Event Bus
        modelBuilder.Entity<OutboxMessage>().ToTable(DbSchemeConsts.Event.Outbox);
        modelBuilder.Entity<InboxMessage>().ToTable(DbSchemeConsts.Event.Inbox);

        // Apply global query filters after all entities are configured in the ModelBuilder
        ApplyGlobalFilters(modelBuilder);
    }
}
