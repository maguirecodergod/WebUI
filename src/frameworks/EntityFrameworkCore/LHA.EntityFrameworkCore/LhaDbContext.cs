using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Base <see cref="DbContext"/> that provides:
/// <list type="bullet">
///   <item>Automatic audit property setting (creation / modification / deletion).</item>
///   <item>Soft-delete handling (converts hard deletes to updates with <c>IsDeleted = true</c>).</item>
///   <item>Optimistic concurrency stamp regeneration.</item>
///   <item>Entity version incrementing.</item>
///   <item>Global query filters for <see cref="ISoftDelete"/> and <see cref="IMultiTenant"/>.</item>
///   <item>Domain event collection forwarded to the current <see cref="IUnitOfWork"/>.</item>
/// </list>
/// </summary>
/// <typeparam name="TDbContext">The concrete DbContext type (CRTP).</typeparam>
public abstract class LhaDbContext<TDbContext> : DbContext, IHasCurrentUnitOfWork
    where TDbContext : DbContext
{
    private readonly IAuditPropertySetter? _auditPropertySetter;
    private readonly ICurrentTenant? _currentTenant;

    /// <summary>
    /// The current <see cref="IUnitOfWork"/>, if one is active.
    /// Used to forward domain events collected during <see cref="SaveChangesAsync"/>.
    /// </summary>
    public IUnitOfWork? CurrentUnitOfWork { get; set; }

    /// <summary>
    /// Whether soft-delete global query filter is enabled. Default: <c>true</c>.
    /// Set to <c>false</c> to include soft-deleted entities in queries.
    /// </summary>
    public bool IsSoftDeleteFilterEnabled { get; set; } = true;

    /// <summary>
    /// Whether multi-tenant global query filter is enabled. Default: <c>true</c>.
    /// Set to <c>false</c> to include entities from all tenants in queries.
    /// </summary>
    public bool IsMultiTenantFilterEnabled { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of <see cref="LhaDbContext{TDbContext}"/>.
    /// </summary>
    /// <param name="options">The EF Core options.</param>
    protected LhaDbContext(DbContextOptions<TDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LhaDbContext{TDbContext}"/> with service injection.
    /// </summary>
    protected LhaDbContext(
        DbContextOptions<TDbContext> options,
        IAuditPropertySetter? auditPropertySetter = null,
        ICurrentTenant? currentTenant = null)
        : base(options)
    {
        _auditPropertySetter = auditPropertySetter;
        _currentTenant = currentTenant;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFilters(modelBuilder, entityType);
        }
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        BeforeSaveChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        AfterSaveChanges();
        return result;
    }

    /// <inheritdoc />
    public override int SaveChanges()
    {
        BeforeSaveChanges();
        var result = base.SaveChanges();
        AfterSaveChanges();
        return result;
    }

    /// <summary>
    /// Applies audit properties, soft-delete handling, concurrency stamps, and entity versions
    /// to all tracked entities before persisting.
    /// </summary>
    protected virtual void BeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();

        // Fix child entities that EF misclassified as Modified.
        // When a new child entity with a pre-set key (e.g., Guid.CreateVersion7()) is added
        // to a tracked navigation collection, EF sees the non-sentinel key and assumes it
        // already exists in the database → marks it as Modified. This causes UPDATE (0 rows)
        // instead of INSERT, throwing DbUpdateConcurrencyException.
        ReclassifyMisclassifiedNewEntities();

        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetGuidIdIfNeeded(entry);
                    SetMultiTenantId(entry);
                    _auditPropertySetter?.SetCreationProperties(entry.Entity);
                    _auditPropertySetter?.IncrementEntityVersionProperty(entry.Entity);
                    break;

                case EntityState.Modified:
                    _auditPropertySetter?.SetModificationProperties(entry.Entity);
                    _auditPropertySetter?.IncrementEntityVersionProperty(entry.Entity);
                    UpdateConcurrencyStamp(entry);
                    break;

                case EntityState.Deleted:
                    HandleSoftDelete(entry);
                    break;
            }
        }
    }

    /// <summary>
    /// Detects entries that EF's change tracker incorrectly marked as <see cref="EntityState.Modified"/>
    /// and reclassifies them as <see cref="EntityState.Added"/>.
    /// <para>
    /// This happens when a new child entity with a pre-set Guid key (non-sentinel) is added to
    /// a tracked navigation collection. EF assumes existing entity → Modified. We detect these
    /// by checking that no property has actually changed (OriginalValue == CurrentValue for all),
    /// which is impossible for a legitimately loaded-then-modified entity.
    /// </para>
    /// <para>
    /// Only non–aggregate-root entities are reclassified. Aggregate roots (which implement
    /// <see cref="IHasConcurrencyStamp"/>) are managed through repositories and their Modified
    /// state is always intentional.
    /// </para>
    /// </summary>
    private void ReclassifyMisclassifiedNewEntities()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified)
                continue;

            // Aggregate roots are updated via repositories — their Modified state is intentional.
            if (entry.Entity is IHasConcurrencyStamp)
                continue;

            // If every non-PK property still has OriginalValue == CurrentValue,
            // this entity was auto-tracked (snapshot == current) and never loaded from DB.
            var hasActualChanges = entry.Properties
                .Where(p => !p.Metadata.IsPrimaryKey())
                .Any(p => !Equals(p.OriginalValue, p.CurrentValue));

            if (!hasActualChanges)
            {
                entry.State = EntityState.Added;
            }
        }
    }

    /// <summary>
    /// Collects domain events from entities and forwards them to the current UoW after save.
    /// </summary>
    protected virtual void AfterSaveChanges()
    {
        var uow = CurrentUnitOfWork;
        if (uow is null) return;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IHasDomainEvents { DomainEvents.Count: > 0 } hasDomainEvents)
            {
                foreach (var domainEvent in hasDomainEvents.DomainEvents)
                {
                    uow.AddLocalEvent(new UnitOfWorkEventRecord(
                        domainEvent.Event.GetType(),
                        domainEvent.Event,
                        UnitOfWorkEventRecord.NextOrder()));
                }

                hasDomainEvents.ClearDomainEvents();
            }
        }
    }

    /// <summary>
    /// Applies global query filters for <see cref="ISoftDelete"/> and <see cref="IMultiTenant"/>.
    /// </summary>
    protected virtual void ConfigureGlobalFilters(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        if (entityType.IsOwned())
            return;

        if (entityType.BaseType is not null)
            return;

        var clrType = entityType.ClrType;

        if (clrType.IsAssignableTo(typeof(ISoftDelete)))
        {
            modelBuilder.Entity(clrType).HasQueryFilter(
                SoftDeleteQueryFilterExpression(clrType));
        }

        if (clrType.IsAssignableTo(typeof(IMultiTenant)))
        {
            modelBuilder.Entity(clrType).HasQueryFilter(
                MultiTenantQueryFilterExpression(clrType));
        }
    }

    /// <summary>
    /// Builds dynamic lambda: <c>e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDeleted</c>.
    /// </summary>
    private System.Linq.Expressions.LambdaExpression SoftDeleteQueryFilterExpression(Type entityType)
    {
        var param = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var dbContext = System.Linq.Expressions.Expression.Constant(this);
        var filterEnabledProp = System.Linq.Expressions.Expression.Property(
            dbContext, nameof(IsSoftDeleteFilterEnabled));
        var isDeleted = System.Linq.Expressions.Expression.Property(
            System.Linq.Expressions.Expression.Convert(param, typeof(ISoftDelete)),
            nameof(ISoftDelete.IsDeleted));

        // !IsSoftDeleteFilterEnabled || !e.IsDeleted
        var body = System.Linq.Expressions.Expression.OrElse(
            System.Linq.Expressions.Expression.Not(filterEnabledProp),
            System.Linq.Expressions.Expression.Not(isDeleted));

        return System.Linq.Expressions.Expression.Lambda(body, param);
    }

    /// <summary>
    /// Builds dynamic lambda: <c>e => !IsMultiTenantFilterEnabled || ((IMultiTenant)e).TenantId == CurrentTenantId</c>.
    /// </summary>
    private System.Linq.Expressions.LambdaExpression MultiTenantQueryFilterExpression(Type entityType)
    {
        var param = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var dbContext = System.Linq.Expressions.Expression.Constant(this);
        var filterEnabledProp = System.Linq.Expressions.Expression.Property(
            dbContext, nameof(IsMultiTenantFilterEnabled));
        var tenantId = System.Linq.Expressions.Expression.Property(
            System.Linq.Expressions.Expression.Convert(param, typeof(IMultiTenant)),
            nameof(IMultiTenant.TenantId));
        var currentTenantId = System.Linq.Expressions.Expression.Property(
            dbContext, nameof(CurrentTenantId));

        // !IsMultiTenantFilterEnabled || e.TenantId == CurrentTenantId
        var body = System.Linq.Expressions.Expression.OrElse(
            System.Linq.Expressions.Expression.Not(filterEnabledProp),
            System.Linq.Expressions.Expression.Equal(tenantId, currentTenantId));

        return System.Linq.Expressions.Expression.Lambda(body, param);
    }

    /// <summary>
    /// The current tenant ID exposed as a property for use in query filter expressions.
    /// </summary>
    public Guid? CurrentTenantId => _currentTenant?.Id;

    /// <summary>
    /// For added entities with a <see cref="Guid"/> key that has the default value,
    /// generates a new v7 GUID.
    /// </summary>
    private static void SetGuidIdIfNeeded(EntityEntry entry)
    {
        if (entry.Entity is IEntity<Guid> entity && entity.Id == Guid.Empty)
        {
            var idProperty = entry.Property(nameof(IEntity<Guid>.Id));
            if (idProperty.Metadata.ValueGenerated == ValueGenerated.Never)
            {
                idProperty.CurrentValue = Guid.CreateVersion7();
            }
        }
    }

    /// <summary>
    /// For added multi-tenant entities, sets the <see cref="IMultiTenant.TenantId"/>
    /// to the current tenant if not already set.
    /// </summary>
    private void SetMultiTenantId(EntityEntry entry)
    {
        if (_currentTenant is null) return;

        if (entry.Entity is IMultiTenant)
        {
            var tenantIdProp = entry.Property(nameof(IMultiTenant.TenantId));
            if (tenantIdProp.CurrentValue is null)
            {
                tenantIdProp.CurrentValue = _currentTenant.Id;
            }
        }
    }

    /// <summary>
    /// Regenerates the <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> on modified entities.
    /// </summary>
    private static void UpdateConcurrencyStamp(EntityEntry entry)
    {
        if (entry.Entity is IHasConcurrencyStamp hasConcurrencyStamp)
        {
            hasConcurrencyStamp.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }
    }

    /// <summary>
    /// Converts a hard delete to a soft delete if the entity implements <see cref="ISoftDelete"/>.
    /// </summary>
    private void HandleSoftDelete(EntityEntry entry)
    {
        if (entry.Entity is not ISoftDelete)
            return;

        entry.Reload();
        entry.State = EntityState.Modified;
        _auditPropertySetter?.SetDeletionProperties(entry.Entity);
        _auditPropertySetter?.IncrementEntityVersionProperty(entry.Entity);
        UpdateConcurrencyStamp(entry);
    }
}
