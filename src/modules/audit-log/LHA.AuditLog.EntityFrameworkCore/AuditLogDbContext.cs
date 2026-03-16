using LHA.Auditing;
using LHA.AuditLog.Domain;
using LHA.EntityFrameworkCore;
using LHA.MultiTenancy;
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
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
    public DbSet<AuditLogActionEntity> AuditLogActions => Set<AuditLogActionEntity>();
    public DbSet<EntityChangeEntity> EntityChanges => Set<EntityChangeEntity>();
    public DbSet<EntityPropertyChangeEntity> EntityPropertyChanges => Set<EntityPropertyChangeEntity>();

    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options)
        : base(options)
    {
    }

    public AuditLogDbContext(
        DbContextOptions<AuditLogDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options, auditPropertySetter, currentTenant)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureAuditLog();
    }
}
