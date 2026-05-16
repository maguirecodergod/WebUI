using System.Text.Json;

namespace LHA.Notification.Infrastructure.Channels.Push.Apns;

public sealed class ApnsMessageBuilder
{
    public (string payload, string path) Build(string deviceToken, string title, string body, string bundleId, Dictionary<string, string>? data)
    {
        var aps = new Dictionary<string, object>
        {
            ["aps"] = new Dictionary<string, object>
            {
                ["alert"] = new Dictionary<string, string>
                {
                    ["title"] = title,
                    ["body"] = body
                },
                ["sound"] = "default",
                ["badge"] = 1
            }
        };

        if (data != null)
        {
            foreach (KeyValuePair<string, string> kvp in data)
            {
                aps[kvp.Key] = kvp.Value;
            }
        }

        string payload = JsonSerializer.Serialize(aps);
        string path = $"/3/device/{deviceToken}";
        return (payload, path);
    }
}
