using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes creation audit fields.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class CreationAuditedEntityDto<TKey> : EntityDto<TKey>, IHasCreationTime, IMayHaveCreator
    where TKey : notnull
{
    /// <inheritdoc />
    public DateTimeOffset CreationTime { get; init; }

    /// <inheritdoc />
    public Guid? CreatorId { get; init; }
}
