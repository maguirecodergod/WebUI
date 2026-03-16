namespace LHA.MessageBroker.Kafka;

/// <summary>
/// Well-known metadata keys for Kafka-specific data in <see cref="PublishResult.BrokerMetadata"/>
/// and <see cref="MessageContext{T}.BrokerMetadata"/>.
/// </summary>
public static class KafkaMetadataKeys
{
    public const string Topic = "kafka.topic";
    public const string Partition = "kafka.partition";
    public const string Offset = "kafka.offset";
}

/// <summary>
/// Extension methods for type-safe access to Kafka-specific metadata
/// in <see cref="PublishResult"/> and <see cref="MessageContext{T}"/>.
/// </summary>
public static class KafkaPublishResultExtensions
{
    /// <summary>Gets the Kafka topic name from the publish result.</summary>
    public static string GetTopic(this PublishResult result)
        => (string)result.BrokerMetadata[KafkaMetadataKeys.Topic];

    /// <summary>Gets the Kafka partition number from the publish result.</summary>
    public static int GetPartition(this PublishResult result)
        => (int)result.BrokerMetadata[KafkaMetadataKeys.Partition];

    /// <summary>Gets the Kafka offset from the publish result.</summary>
    public static long GetOffset(this PublishResult result)
        => (long)result.BrokerMetadata[KafkaMetadataKeys.Offset];
}

/// <summary>
/// Extension methods for type-safe access to Kafka-specific metadata in <see cref="MessageContext{T}"/>.
/// </summary>
public static class KafkaMessageContextExtensions
{
    /// <summary>Gets the Kafka topic name from the message context.</summary>
    public static string GetTopic<T>(this MessageContext<T> context) where T : class
        => (string)context.BrokerMetadata[KafkaMetadataKeys.Topic];

    /// <summary>Gets the Kafka partition number from the message context.</summary>
    public static int GetPartition<T>(this MessageContext<T> context) where T : class
        => (int)context.BrokerMetadata[KafkaMetadataKeys.Partition];

    /// <summary>Gets the Kafka offset from the message context.</summary>
    public static long GetOffset<T>(this MessageContext<T> context) where T : class
        => (long)context.BrokerMetadata[KafkaMetadataKeys.Offset];
}
