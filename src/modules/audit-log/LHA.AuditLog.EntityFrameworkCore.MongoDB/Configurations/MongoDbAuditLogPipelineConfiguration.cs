using LHA.AuditLog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.AuditLog.EntityFrameworkCore.MongoDB.Configurations;

public sealed class MongoDbAuditLogPipelineConfiguration : IEntityTypeConfiguration<AuditLogPipelineEntity>
{
    public void Configure(EntityTypeBuilder<AuditLogPipelineEntity> b)
    {
        b.ToTable(AuditLogDbConsts.AuditLogPipeline);
        b.HasKey(e => e.Id);
        
        b.Property(e => e.Id).HasMaxLength(26);
        b.Property(e => e.ServiceName).HasMaxLength(256);
        b.Property(e => e.InstanceId).HasMaxLength(256);
        b.Property(e => e.ActionName).HasMaxLength(512);
        b.Property(e => e.UserId).HasMaxLength(40);
        b.Property(e => e.TenantId).HasMaxLength(40);
        b.Property(e => e.UserName).HasMaxLength(256);
        b.Property(e => e.TraceId).HasMaxLength(64);
        b.Property(e => e.SpanId).HasMaxLength(64);
        b.Property(e => e.CorrelationId).HasMaxLength(128);
        b.Property(e => e.HttpMethod).HasMaxLength(10);
        b.Property(e => e.RequestPath).HasMaxLength(2048);
        b.Property(e => e.ClientIp).HasMaxLength(64);
        b.Property(e => e.UserAgent).HasMaxLength(512);
    }
}
