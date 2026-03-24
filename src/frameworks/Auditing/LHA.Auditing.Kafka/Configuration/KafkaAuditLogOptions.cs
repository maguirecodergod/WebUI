namespace LHA.Auditing.Kafka;

/// <summary>
/// Configuration options for the Kafka audit log dispatcher.
/// </summary>
public sealed class KafkaAuditLogOptions
{
    /// <summary>
    /// Kafka topic for audit logs. Default: "audit-logs".
    /// </summary>
    public string TopicName { get; set; } = "audit-logs";

    /// <summary>
    /// Whether to use the TenantId as the Kafka partition key.
    /// Ensures all logs for a tenant go to the same partition (ordered).
    /// Default: <c>true</c>.
    /// </summary>
    public bool PartitionByTenant { get; set; } = true;
}
