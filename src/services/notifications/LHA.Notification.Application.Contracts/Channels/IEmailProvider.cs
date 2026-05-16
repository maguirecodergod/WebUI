namespace LHA.Notification.Application.Contracts;

public interface IEmailProvider : INotificationChannelProvider
{
    Task<bool> SendEmailAsync(string recipientEmail, Guid tenantId, string subject, string body, bool isHtml, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateEmailAsync(string recipientEmail, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, string? locale = null, CancellationToken cancellationToken = default);
    Task<bool> SendBulkEmailAsync(IEnumerable<string> recipientEmails, Guid tenantId, string subject, string body, bool isHtml, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
}

public interface ISendGridEmailProvider : IEmailProvider { }
public interface IAwsSesEmailProvider : IEmailProvider { }
public interface ISmtpEmailProvider : IEmailProvider { }
