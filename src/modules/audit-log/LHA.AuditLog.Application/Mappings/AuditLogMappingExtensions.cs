using LHA.AuditLog.Domain;

namespace LHA.AuditLog.Application.Mappings;

public static class AuditLogMappingExtensions
{
    public static AuditLogDto MapToDto(this AuditLogEntity entity)
    {
        var auditLogDto = new AuditLogDto
        {
            Id = entity.Id,
            ApplicationName = entity.ApplicationName,
            UserId = entity.UserId,
            UserName = entity.UserName,
            TenantId = entity.TenantId,
            TenantName = entity.TenantName,
            ImpersonatorUserId = entity.ImpersonatorUserId,
            ImpersonatorTenantId = entity.ImpersonatorTenantId,
            ExecutionTime = entity.ExecutionTime,
            ExecutionDuration = entity.ExecutionDuration,
            ClientId = entity.ClientId,
            CorrelationId = entity.CorrelationId,
            ClientIpAddress = entity.ClientIpAddress,
            HttpMethod = entity.HttpMethod,
            HttpStatusCode = entity.HttpStatusCode,
            RequestType = entity.RequestType,
            Url = entity.Url,
            BrowserInfo = entity.BrowserInfo,
            Exceptions = entity.Exceptions,
            Comments = entity.Comments,
            ExtraProperties = entity.ExtraProperties,
            Actions = entity.Actions.Select(a => new AuditLogActionDto
            {
                Id = a.Id,
                ServiceName = a.ServiceName,
                MethodName = a.MethodName,
                Parameters = a.Parameters,
                ExecutionTime = a.ExecutionTime,
                ExecutionDuration = a.ExecutionDuration
            }).ToList(),
            EntityChanges = entity.EntityChanges.Select(x => x.MapToDto()).ToList()
        };

        return auditLogDto;
    }

    public static AuditLogActionDto MapToDto(this AuditLogActionEntity entity)
    {
        return new AuditLogActionDto
        {
            Id = entity.Id,
            ServiceName = entity.ServiceName,
            MethodName = entity.MethodName,
            Parameters = entity.Parameters,
            ExecutionTime = entity.ExecutionTime,
            ExecutionDuration = entity.ExecutionDuration
        };
    }

    public static EntityChangeDto MapToDto(this EntityChangeEntity entity)
    {
        return new EntityChangeDto
        {
            Id = entity.Id,
            AuditLogId = entity.AuditLogId,
            ChangeTime = entity.ChangeTime,
            ChangeType = entity.ChangeType,
            EntityTenantId = entity.EntityTenantId,
            EntityId = entity.EntityId,
            EntityTypeFullName = entity.EntityTypeFullName,
            PropertyChanges = entity.PropertyChanges.Select(p => new EntityPropertyChangeDto
            {
                Id = p.Id,
                PropertyName = p.PropertyName,
                PropertyTypeFullName = p.PropertyTypeFullName,
                OriginalValue = p.OriginalValue,
                NewValue = p.NewValue
            }).ToList()
        };
    }

    public static EntityPropertyChangeDto MapToDto(this EntityPropertyChangeEntity entity)
    {
        return new EntityPropertyChangeDto
        {
            Id = entity.Id,
            PropertyName = entity.PropertyName,
            PropertyTypeFullName = entity.PropertyTypeFullName,
            OriginalValue = entity.OriginalValue,
            NewValue = entity.NewValue
        };
    }
}
