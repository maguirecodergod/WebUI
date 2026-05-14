using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain.ValueObjects;

public sealed class ChannelPreference
{
    public CNotificationChannel Channel { get; private set; }
    public bool Enabled { get; private set; }
    public List<string> Categories { get; private set; } = new();

    public ChannelPreference(
        CNotificationChannel channel,
        bool enabled,
        List<string> categories)
    {
        Channel = channel;
        Enabled = enabled;
        Categories = categories;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    public void AddCategory(string category)
    {
        if (!Categories.Contains(category))
        {
            Categories.Add(category);
        }
    }

    public void RemoveCategory(string category)
    {
        Categories.Remove(category);
    }
}