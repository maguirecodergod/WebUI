using LHA.Notification.Domain.Shared;
using LHA.Notification.Domain.ValueObjects;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.Notification.Domain;

public sealed class TemplateEntity : FullAuditedEntity<Guid>,
    IMultiTenant
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public CNotificationType Type { get; private set; }
    public List<CNotificationChannel> SupportedChannels { get; private set; } = new();
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }
    public List<TemplateVersionEntity> Versions { get; private set; } = new();
    public string DefaultLocale { get; private set; } = "en";
    public List<string> Tags { get; private set; } = new();

    public Guid? TenantId { get; private set; }


    private TemplateEntity()
    {
    }

    public TemplateEntity(
        string code,
        string name,
        string description,
        CNotificationType type,
        string defaultLocale)
    {
        Code = code;
        Name = name;
        Description = description;
        Type = type;
        DefaultLocale = defaultLocale;
        IsActive = true;
        IsSystem = false;
    }

    public void AddVersion(TemplateVersionEntity version)
    {
        var existingVersion = Versions.FirstOrDefault(v => v.Locale == version.Locale && v.Channel == version.Channel);
        if (existingVersion == null)
        {
            Versions.Add(version);
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void SetSystem(bool isSystem)
    {
        IsSystem = isSystem;
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateSupportedChannels(List<CNotificationChannel> channels)
    {
        SupportedChannels = channels;
    }

    public void UpdateDefaultLocale(string locale)
    {
        DefaultLocale = locale;
    }

    public void UpdateTags(List<string> tags)
    {
        Tags = tags;
    }
}