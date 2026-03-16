namespace LHA.MessageBroker;

/// <summary>
/// Defines how tenants are isolated across message destinations.
/// Critical design decision for multi-tenant SaaS at scale.
/// Semantics differ slightly per broker but intent is consistent.
/// </summary>
public enum TenantTopicStrategy
{
    /// <summary>
    /// All tenants share the same destination. Tenant ID is in message headers.
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> shared topic, no partitioning by tenant.</item>
    ///   <item><b>RabbitMQ:</b> shared exchange, tenant ID in headers only.</item>
    /// </list>
    /// Pros: Simple, fewer resources. Cons: Noisy neighbor, no isolation.
    /// Best for: Low-volume tenants, ≤10k tenants.
    /// </summary>
    SharedTopic = 0,

    /// <summary>
    /// Each tenant gets a dedicated destination: "{prefix}.{tenantId}.{baseName}".
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> tenant-specific topic.</item>
    ///   <item><b>RabbitMQ:</b> tenant-specific exchange.</item>
    /// </list>
    /// Pros: Full isolation, independent scaling. Cons: Resource explosion at scale.
    /// Best for: Enterprise/premium tenants requiring strict isolation.
    /// </summary>
    TenantPrefixTopic = 1,

    /// <summary>
    /// All tenants share the same destination but are routed to specific partitions/queues
    /// using tenant ID as the routing key (consistent hashing).
    /// <list type="bullet">
    ///   <item><b>Kafka:</b> tenant ID as partition key for co-location.</item>
    ///   <item><b>RabbitMQ:</b> tenant ID prefixed to routing key for queue binding.</item>
    /// </list>
    /// Pros: Balanced isolation + scalability. Cons: Partition/queue limits.
    /// Best for: High-volume SaaS with 10k–1M tenants.
    /// </summary>
    TenantPartition = 2
}
