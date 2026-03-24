using System.Text.Json;
using LHA.Auditing;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LHA.EntityFrameworkCore.Auditing;

/// <summary>
/// Entity Framework Core interceptor that captures entity changes and pushes
/// them to the <see cref="IAuditingManager"/>'s current scope for Data Auditing.
/// </summary>
public sealed class DataAuditingSaveChangesInterceptor(IAuditingManager auditingManager) : SaveChangesInterceptor
{
    private readonly IAuditingManager _auditingManager = auditingManager;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ProcessAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ProcessAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessAuditLogs(DbContext? context)
    {
        if (context is null) return;
        
        var currentScope = _auditingManager.Current;
        if (currentScope?.Log is null) return;

        context.ChangeTracker.DetectChanges();
        
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var entityType = entry.Entity.GetType();

            var changeType = entry.State switch
            {
                EntityState.Added => CEntityChangeType.Created,
                EntityState.Modified => CEntityChangeType.Updated,
                EntityState.Deleted => CEntityChangeType.Deleted,
                _ => CEntityChangeType.Updated
            };

            var entityIdJson = GetEntityId(entry);

            var entityChange = new EntityChangeEntry
            {
                ChangeTime = DateTimeOffset.UtcNow,
                ChangeType = changeType,
                EntityTypeFullName = entityType.FullName,
                EntityTenantId = entry.Entity is IMultiTenant mt ? mt.TenantId : null,
                EntityId = entityIdJson,
                PropertyChanges = GetPropertyChanges(entry)
            };

            currentScope.Log.EntityChanges.Add(entityChange);
        }
    }

    private static string? GetEntityId(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();
        if (pk is null) return null;

        var dict = new Dictionary<string, object?>();
        foreach (var prop in pk.Properties)
        {
            dict[prop.Name] = entry.Property(prop.Name).CurrentValue;
        }
        return JsonSerializer.Serialize(dict);
    }

    private static List<EntityPropertyChange> GetPropertyChanges(EntityEntry entry)
    {
        var changes = new List<EntityPropertyChange>();

        foreach (var property in entry.Properties)
        {
            if (property.IsTemporary) continue;
            
            bool isModified = false;
            if (entry.State == EntityState.Added)
            {
                isModified = true;
            }
            else if (entry.State == EntityState.Modified && property.IsModified)
            {
                isModified = true;
            }
            else if (entry.State == EntityState.Deleted)
            {
                isModified = true;
            }

            if (isModified)
            {
                var propChange = new EntityPropertyChange
                {
                    PropertyName = property.Metadata.Name,
                    PropertyTypeFullName = property.Metadata.ClrType.FullName,
                    NewValue = entry.State != EntityState.Deleted ? SerializeVal(property.CurrentValue) : null,
                    OriginalValue = entry.State != EntityState.Added ? SerializeVal(property.OriginalValue) : null
                };
                changes.Add(propChange);
            }
        }

        return changes;
    }

    private static string? SerializeVal(object? val)
    {
        if (val is null) return null;
        if (val is string s) return s;
        if (val.GetType().IsPrimitive || val is Guid || val is DateTime || val is DateTimeOffset || val is decimal)
            return val.ToString();
        
        return JsonSerializer.Serialize(val);
    }
}
