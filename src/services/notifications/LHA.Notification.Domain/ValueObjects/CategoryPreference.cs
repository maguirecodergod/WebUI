using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.ValueObjects;

public sealed class CategoryPreference
{
    public string Category { get; private set; }
    public bool Enabled { get; private set; }
    public List<CNotificationChannel> Channels { get; private set; } = new();

    public CategoryPreference(
        string category,
        bool enabled,
        List<CNotificationChannel> channels)
    {
        Category = category;
        Enabled = enabled;
        Channels = channels;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public void AddChannel(CNotificationChannel channel)
    {
        if (!Channels.Contains(channel))
        {
            Channels.Add(channel);
        }
    }

    public void RemoveChannel(CNotificationChannel channel)
    {
        Channels.Remove(channel);
    }
}
