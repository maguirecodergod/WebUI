using System.Text.Json.Serialization;
using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes full audit fields (creation, modification, and soft-delete) grouped into value objects.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class FullAuditedEntityDto<TKey> : AuditedEntityDto<TKey>, IFullAuditedObject
    where TKey : notnull
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Information about the deletion event.
    /// </summary>
    public AuditActionDto? Deletion { get; set; }

    [JsonIgnore]
    public DateTimeOffset? DeletionTime 
    { 
        get => Deletion?.Time; 
        set 
        {
            if (value.HasValue)
            {
                Deletion ??= new AuditActionDto();
                Deletion.Time = value;
            }
        }
    }

    [JsonIgnore]
    public Guid? DeleterId 
    { 
        get => Deletion?.Actor?.Id; 
        set 
        {
            if (value.HasValue)
            {
                Deletion ??= new AuditActionDto();
                Deletion.Actor ??= new AuditorDto();
                Deletion.Actor.Id = value;
            }
        }
    }
}
