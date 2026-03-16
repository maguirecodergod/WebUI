using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes creation and modification audit fields.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class AuditedEntityDto<TKey> : CreationAuditedEntityDto<TKey>, IModificationAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset? LastModificationTime { get; init; }

    /// <inheritdoc />
    public Guid? LastModifierId { get; init; }
}
