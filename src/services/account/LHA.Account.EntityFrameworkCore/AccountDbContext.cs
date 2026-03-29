using LHA.Auditing;
using LHA.AuditLog.EntityFrameworkCore;
using LHA.EntityFrameworkCore;
using LHA.Identity.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.PermissionManagement.EntityFrameworkCore;
using LHA.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.Account.EntityFrameworkCore;

/// <summary>
/// Unified DbContext for the Account Service, combining all module entity mappings
/// into a single context. This replaces the individual module DbContexts at runtime
/// via the ReplaceDbContext pattern, so all modules share one connection and transaction.
/// </summary>
public sealed class AccountDbContext
    : LhaDbContext<AccountDbContext>, IHasEventOutbox, IHasEventInbox
{
    /// <inheritdoc />
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    private readonly IServiceProvider _serviceProvider;

    public AccountDbContext(
        DbContextOptions<AccountDbContext> options,
        IServiceProvider serviceProvider,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Apply Data Auditing interceptor to capture EF Core entity changes
        var interceptor = _serviceProvider?.GetService<LHA.EntityFrameworkCore.Auditing.DataAuditingSaveChangesInterceptor>();
        if (interceptor is not null)
        {
            optionsBuilder.AddInterceptors(interceptor);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Determine the audit mode configured in DI (fallback to All)
        var auditOptions = _serviceProvider?.GetService<Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>>();
        var auditMode = auditOptions?.Value.Mode ?? AuditLogStoreMode.All;

        // Apply module model configurations first.
        modelBuilder.ConfigureIdentity();
        modelBuilder.ConfigureTenantManagement();
        modelBuilder.ConfigureAuditLog(auditMode);
        modelBuilder.ConfigurePermissionManagement();
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
    }
}
