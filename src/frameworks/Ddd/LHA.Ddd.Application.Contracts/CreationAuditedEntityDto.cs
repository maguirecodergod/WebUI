using System.Text.Json.Serialization;
using LHA.Auditing;

namespace LHA.Ddd.Application;

/// <summary>
/// DTO that includes creation audit fields grouped into a single value object.
/// </summary>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class CreationAuditedEntityDto<TKey> : EntityDto<TKey>, ICreationAuditedObject
    where TKey : notnull
{
    /// <summary>
    /// Information about the creation event.
    /// </summary>
    public AuditActionDto Creation { get; set; } = new();

    [JsonIgnore]
    public DateTimeOffset CreationTime 
    { 
        get => Creation.Time ?? DateTimeOffset.UtcNow; 
        set => Creation.Time = value; 
    }

    [JsonIgnore]
    public Guid? CreatorId 
    { 
        get => Creation.Actor?.Id; 
        set 
        {
            if (value.HasValue)
            {
                Creation.Actor ??= new AuditorDto();
                Creation.Actor.Id = value;
            }
        }
    }
}
