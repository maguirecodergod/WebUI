using LHA.Notification.Domain.Shared;
using LHA.Notification.Domain.ValueObjects;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;

namespace LHA.Notification.Domain;

public sealed class UserPreferenceEntity : FullAuditedEntity<Guid>,
    IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public Guid UserId { get; private set; } = default!;
    public bool GlobalOptOut { get; private set; }
    public List<ChannelPreference> Channels { get; private set; } = new();
    public List<CategoryPreference> Categories { get; private set; } = new();
    public QuietHoursSettings? QuietHours { get; private set; }
    public string Timezone { get; private set; } = default!;
    public string Locale { get; private set; } = "en";

    public UserPreferenceEntity()
    {
    }

    public UserPreferenceEntity(Guid userId)
    {
        UserId = userId;
        Timezone = "UTC";
        Locale = "en";
        GlobalOptOut = false;
    }

    public void OptOut(bool optOut)
    {
        GlobalOptOut = optOut;
    }

    public void SetChannelPreference(CNotificationChannel channel, bool enabled, List<string> categories)
    {
        var existing = Channels.FirstOrDefault(c => c.Channel == channel);
        if (existing == null)
        {
            Channels.Add(new ChannelPreference(channel, enabled, categories));
        }
        else
        {
            if (enabled)
            {
                existing.Enable();
            }
            else
            {
                existing.Disable();
            }
            categories.ForEach(c => existing.AddCategory(c));
        }
    }

    public void SetCategoryPreference(string category, bool enabled, List<CNotificationChannel> channels)
    {
        var existing = Categories.FirstOrDefault(c => c.Category == category);
        if (existing == null)
        {
            Categories.Add(new CategoryPreference(category, enabled, channels));
        }
        else
        {
            if (enabled)
            {
                existing.Enable();
            }
            else
            {
                existing.Disable();
            }
            channels.ForEach(c => existing.AddChannel(c));
        }
    }

    public void SetQuietHours(QuietHoursSettings quietHours)
    {
        QuietHours = quietHours;
    }

    public void SetTimezone(string timezone)
    {
        Timezone = timezone;
    }

    public void SetLocale(string locale)
    {
        Locale = locale;
    }
}