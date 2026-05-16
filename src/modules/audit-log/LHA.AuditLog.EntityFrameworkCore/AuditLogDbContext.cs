using LHA.Auditing;
using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
using LHA.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// EF Core DbContext for the Audit Log module.
/// <para>
/// Does NOT inherit <see cref="IHasEventOutbox"/> or <see cref="IHasEventInbox"/>
/// because audit logs are write-once, fire-and-forget records with no domain events.
/// </para>
/// </summary>
public class AuditLogDbContext : LhaDbContext<AuditLogDbContext>
{
    // ── Data Audit (relational, structured) ────────────────────────────
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
    public DbSet<AuditLogActionEntity> AuditLogActions => Set<AuditLogActionEntity>();
    public DbSet<EntityChangeEntity> EntityChanges => Set<EntityChangeEntity>();
    public DbSet<EntityPropertyChangeEntity> EntityPropertyChanges => Set<EntityPropertyChangeEntity>();

    // ── Pipeline Audit (high-throughput, lightweight) ──────────────────
    public DbSet<AuditLogPipelineEntity> AuditLogPipeline => Set<AuditLogPipelineEntity>();

    private readonly Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? _options;

    // Use only the constructor with IOptions to ensure ModelConfigurator is correctly injected


    public AuditLogDbContext(
        DbContextOptions<AuditLogDbContext> options,
        Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? auditOptions = null,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null,
        ICurrentUser? currentUser = null)
        : base(options, auditPropertySetter, currentTenant, currentUser)
    {
        _options = auditOptions;

        if (_options?.Value.AutoTransactionBehavior.HasValue == true)
        {
            Database.AutoTransactionBehavior = _options.Value.AutoTransactionBehavior.Value;
        }
    }

    /// <summary>
    /// Design-time configurator used by <c>IDesignTimeDbContextFactory</c> when DI is not available.
    /// </summary>
    public static Action<ModelBuilder>? DesignTimeModelConfigurator { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var configurator = _options?.Value.ModelConfigurator ?? DesignTimeModelConfigurator;
        configurator?.Invoke(modelBuilder);
        
        // Apply global query filters (IMultiTenant, ISoftDelete) after entities are added
        ApplyGlobalFilters(modelBuilder);
    }
}
