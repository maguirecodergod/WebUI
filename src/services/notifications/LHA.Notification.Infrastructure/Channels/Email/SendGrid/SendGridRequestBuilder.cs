using System.Text.Json;


namespace LHA.Notification.Infrastructure.Channels.Email.SendGrid;

public sealed class SendGridRequestBuilder
{
    public object BuildRequest(string toEmail, string subject, string body, string? htmlBody, string? notificationId, SendGridProviderSettings settings)
    {
        var fromEmail = settings.FromEmail ?? "noreply@example.com";
        var fromName = settings.FromName ?? "Notification Service";
        
        var request = new
        {
            personalizations = new[]
            {
                new { to = new[] { new { email = toEmail } } }
            },
            from = new { email = fromEmail, name = fromName },
            reply_to = string.IsNullOrEmpty(settings.ReplyToEmail) ? null : new { email = settings.ReplyToEmail, name = settings.ReplyToName },
            subject,
            content = new[]
            {
                new { type = "text/plain", value = body },
                new { type = "text/html", value = htmlBody ?? body }
            },
            custom_args = string.IsNullOrEmpty(notificationId)
                ? null
                : new { notification_id = notificationId },
            mail_settings = new
            {
                bypass_list_management = new { enable = settings.BypassListManagement },
                sandbox_mode = new { enable = settings.SandboxMode }
            },
            tracking_settings = new
            {
                click_tracking = new { enable = settings.ClickTracking, enable_text = false },
                open_tracking = new { enable = settings.OpenTracking },
                subscription_tracking = new { enable = settings.SubscriptionTracking }
            }
        };
        return request;
    }

    public object BuildBatchRequest(IEnumerable<string> toEmails, string subject, string body, string? htmlBody, string? notificationId, SendGridProviderSettings settings)
    {
        var fromEmail = settings.FromEmail ?? "noreply@example.com";
        var fromName = settings.FromName ?? "Notification Service";

        var personalizations = toEmails.Select(email => new { to = new[] { new { email } } }).ToArray();
        var request = new
        {
            personalizations,
            from = new { email = fromEmail, name = fromName },
            reply_to = string.IsNullOrEmpty(settings.ReplyToEmail) ? null : new { email = settings.ReplyToEmail, name = settings.ReplyToName },
            subject,
            content = new[]
            {
                new { type = "text/plain", value = body },
                new { type = "text/html", value = htmlBody ?? body }
            },
            custom_args = string.IsNullOrEmpty(notificationId)
                ? null
                : new { notification_id = notificationId },
            mail_settings = new
            {
                bypass_list_management = new { enable = settings.BypassListManagement },
                sandbox_mode = new { enable = settings.SandboxMode }
            },
            tracking_settings = new
            {
                click_tracking = new { enable = settings.ClickTracking, enable_text = false },
                open_tracking = new { enable = settings.OpenTracking },
                subscription_tracking = new { enable = settings.SubscriptionTracking }
            }
        };
        return request;
    }
}
