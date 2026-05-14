namespace LHA.Notification.Application.Contracts;

public interface ISmsProvider : INotificationChannelProvider
{
    Task<bool> SendSmsAsync(string recipientPhone, string message, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateSmsAsync(string recipientPhone, Guid templateId, Dictionary<string, object>? variables, string? locale, CancellationToken cancellationToken = default);
    Task<bool> SendBulkSmsAsync(IEnumerable<string> recipientPhones, string message, CancellationToken cancellationToken = default);
    Task<bool> SendVerificationCodeAsync(string recipientPhone, string verificationCode, string tenantName, CancellationToken cancellationToken = default);
    Task<bool> SendOtpSmsAsync(string recipientPhone, string otp, string purpose, CancellationToken cancellationToken = default);
}

public interface ITwilioSmsProvider : ISmsProvider { }
public interface IAwsSnsSmsProvider : ISmsProvider { }