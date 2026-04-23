using System.Text.Json.Serialization;
using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes creation and modification audit fields grouped into value objects.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class AuditedEntityDto<TKey> : CreationAuditedEntityDto<TKey>, IModificationAuditedObject
    where TKey : notnull
{
    /// <summary>
    /// Information about the last modification event.
    /// </summary>
    public AuditActionDto? LastModification { get; set; }

    [JsonIgnore]
    public DateTimeOffset? LastModificationTime 
    { 
        get => LastModification?.Time; 
        set 
        {
            if (value.HasValue)
            {
                LastModification ??= new AuditActionDto();
                LastModification.Time = value;
            }
        }
    }

    [JsonIgnore]
    public Guid? LastModifierId 
    { 
        get => LastModification?.Actor?.Id; 
        set 
        {
            if (value.HasValue)
            {
                LastModification ??= new AuditActionDto();
                LastModification.Actor ??= new AuditorDto();
                LastModification.Actor.Id = value;
            }
        }
    }
}
