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
public sealed class AuditLogDbContext : LhaDbContext<AuditLogDbContext>
{
    // ── Data Audit (relational, structured) ────────────────────────────
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
    public DbSet<AuditLogActionEntity> AuditLogActions => Set<AuditLogActionEntity>();
    public DbSet<EntityChangeEntity> EntityChanges => Set<EntityChangeEntity>();
    public DbSet<EntityPropertyChangeEntity> EntityPropertyChanges => Set<EntityPropertyChangeEntity>();

    // ── Pipeline Audit (high-throughput, lightweight) ──────────────────
    public DbSet<AuditLogPipelineEntity> AuditLogPipeline => Set<AuditLogPipelineEntity>();

    private readonly Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? _options;

    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options)
        : base(options)
    {
    }

    public AuditLogDbContext(
        DbContextOptions<AuditLogDbContext> options,
        Microsoft.Extensions.Options.IOptions<AuditLogEntityFrameworkCoreOptions>? auditOptions = null,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null,
        ICurrentUser? currentUser = null)
        : base(options, auditPropertySetter, currentTenant, currentUser)
    {
        _options = auditOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureAuditLog(_options?.Value.Mode ?? AuditLogStoreMode.All);
        
        // Apply global query filters (IMultiTenant, ISoftDelete) after entities are added
        ApplyGlobalFilters(modelBuilder);
    }
}
