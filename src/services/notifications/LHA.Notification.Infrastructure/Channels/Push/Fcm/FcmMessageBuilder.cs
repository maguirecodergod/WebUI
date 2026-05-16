namespace LHA.Notification.Infrastructure.Channels.Push.Fcm;

public sealed class FcmMessageBuilder
{
    public object BuildSingleMessage(string token, string title, string body, Dictionary<string, string>? data)
    {
        var notification = new
        {
            token,
            notification = new { title, body },
            data = data ?? new Dictionary<string, string>(),
            android = new
            {
                priority = "high",
                notification = new { click_action = "OPEN_ACTIVITY" }
            },
            apns = new
            {
                payload = new { aps = new { sound = "default", badge = 1 } }
            }
        };

        return notification;
    }

    public List<object> BuildBatchMessages(IEnumerable<string> tokens, string title, string body, Dictionary<string, string>? data)
    {
        return tokens.Select(token => BuildSingleMessage(token, title, body, data)).ToList();
    }
}
