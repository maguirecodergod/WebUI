using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes full audit fields (creation, modification, and soft-delete).
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class FullAuditedEntityDto<TKey> : AuditedEntityDto<TKey>, IFullAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public bool IsDeleted { get; init; }

    /// <inheritdoc />
    public DateTimeOffset? DeletionTime { get; init; }

    /// <inheritdoc />
    public Guid? DeleterId { get; init; }
}
