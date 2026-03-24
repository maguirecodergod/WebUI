namespace LHA.Auditing.EfCore
{
    /// <summary>
    /// Lightweight EF Core entity for the pipeline audit log records.
    /// Separate from the existing AuditLogEntity to avoid coupling.
    /// </summary>
    public sealed class AuditLogPipelineEntity
    {
        public string Id { get; set; } = default!;
        public DateTimeOffset Timestamp { get; set; }
        public long DurationMs { get; set; }
        public string? ServiceName { get; set; }
        public string? InstanceId { get; set; }
        public string? ActionName { get; set; }
        public byte ActionType { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public string? UserName { get; set; }
        public string? Roles { get; set; }
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }
        public string? CorrelationId { get; set; }
        public byte Status { get; set; }
        public int? StatusCode { get; set; }
        public string? HttpMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? ClientIp { get; set; }
        public string? UserAgent { get; set; }
        public string? Exception { get; set; }
        public string? Tags { get; set; }
    }
}