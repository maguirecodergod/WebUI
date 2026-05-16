namespace LHA.Notification.Domain.Shared;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDB";
    public required string DatabaseName { get; set; }
    public string? ReplicaSetName { get; set; }
}
