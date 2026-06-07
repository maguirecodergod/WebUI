using LHA.Ddd.Domain;

namespace LHA.AuditLog.Domain
{
    public interface IAuditLogPipelineRepository : IRepository<AuditLogPipelineEntity, Guid>
    {
        
    }
}