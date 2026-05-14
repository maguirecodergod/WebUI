namespace LHA.Notification.Application.Contracts;

public interface IEmailProvider : INotificationChannelProvider
{
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string body, bool isHtml, Dictionary<string, string>? metadata, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateEmailAsync(string recipientEmail, Guid templateId, Dictionary<string, object>? variables, string? locale, CancellationToken cancellationToken = default);
    Task<bool> SendBulkEmailAsync(IEnumerable<string> recipientEmails, string subject, string body, bool isHtml, Dictionary<string, string>? metadata, CancellationToken cancellationToken = default);
    Task<bool> SendWelcomeEmailAsync(string recipientEmail, string userName, string tenantName, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetEmailAsync(string recipientEmail, string resetToken, string userName, string tenantName, CancellationToken cancellationToken = default);
}

public interface ISendGridEmailProvider : IEmailProvider { }
public interface IAwsSesEmailProvider : IEmailProvider { }
public interface ISmtpEmailProvider : IEmailProvider { }