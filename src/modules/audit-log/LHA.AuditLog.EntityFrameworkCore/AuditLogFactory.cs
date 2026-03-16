using System.Text.Json;
using LHA.Auditing;
using LHA.AuditLog.Domain;

namespace LHA.AuditLog.EntityFrameworkCore;

/// <summary>
/// Factory that maps the framework's in-memory <see cref="AuditLogEntry"/>
/// to persistent <see cref="AuditLogEntity"/> and its sub-entities.
/// </summary>
public static class AuditLogFactory
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Creates a full <see cref="AuditLogEntity"/> graph from a framework <see cref="AuditLogEntry"/>.
    /// </summary>
    public static AuditLogEntity Create(AuditLogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var logId = Guid.CreateVersion7();

        var exceptions = entry.Exceptions.Count > 0
            ? JsonSerializer.Serialize(entry.Exceptions.Select(e => e.ToString()), JsonOptions)
            : null;

        var comments = entry.Comments.Count > 0
            ? JsonSerializer.Serialize(entry.Comments, JsonOptions)
            : null;

        var extraProperties = entry.ExtraProperties.Count > 0
            ? JsonSerializer.Serialize(entry.ExtraProperties, JsonOptions)
            : null;

        var auditLog = new AuditLogEntity(
            id: logId,
            applicationName: entry.ApplicationName,
            userId: entry.UserId,
            userName: entry.UserName,
            tenantId: entry.TenantId,
            tenantName: entry.TenantName,
            impersonatorUserId: entry.ImpersonatorUserId,
            impersonatorTenantId: entry.ImpersonatorTenantId,
            executionTime: entry.ExecutionTime,
            executionDuration: entry.ExecutionDuration,
            clientId: entry.ClientId,
            correlationId: entry.CorrelationId,
            clientIpAddress: entry.ClientIpAddress,
            httpMethod: entry.HttpMethod,
            httpStatusCode: entry.HttpStatusCode,
            url: entry.Url,
            browserInfo: entry.BrowserInfo,
            exceptions: exceptions,
            comments: comments,
            extraProperties: extraProperties);

        // Map actions
        foreach (var action in entry.Actions)
        {
            auditLog.AddAction(new AuditLogActionEntity(
                auditLogId: logId,
                tenantId: entry.TenantId,
                serviceName: action.ServiceName,
                methodName: action.MethodName,
                parameters: action.Parameters,
                executionTime: action.ExecutionTime,
                executionDuration: action.ExecutionDuration));
        }

        // Map entity changes with property changes
        foreach (var change in entry.EntityChanges)
        {
            var entityChange = new EntityChangeEntity(
                auditLogId: logId,
                tenantId: entry.TenantId,
                changeTime: change.ChangeTime,
                changeType: change.ChangeType,
                entityTenantId: change.EntityTenantId,
                entityId: change.EntityId,
                entityTypeFullName: change.EntityTypeFullName);

            foreach (var propChange in change.PropertyChanges)
            {
                entityChange.AddPropertyChange(new EntityPropertyChangeEntity(
                    entityChangeId: entityChange.Id,
                    tenantId: entry.TenantId,
                    propertyName: propChange.PropertyName,
                    propertyTypeFullName: propChange.PropertyTypeFullName,
                    originalValue: propChange.OriginalValue,
                    newValue: propChange.NewValue));
            }

            auditLog.AddEntityChange(entityChange);
        }

        return auditLog;
    }
}
