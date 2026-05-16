using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Application.Contracts;

public interface ISmsProvider : INotificationChannelProvider
{
    Task<bool> SendSmsAsync(string recipientPhone, Guid tenantId, string message, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateSmsAsync(string recipientPhone, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, string? locale = null, CancellationToken cancellationToken = default);
    Task<bool> SendBulkSmsAsync(IEnumerable<string> recipientPhones, Guid tenantId, string message, CancellationToken cancellationToken = default);
    Task<bool> SendVerificationCodeAsync(string recipientPhone, Guid tenantId, string verificationCode, string tenantName, CancellationToken cancellationToken = default);
    Task<bool> SendOtpSmsAsync(string recipientPhone, Guid tenantId, string otp, string purpose, CancellationToken cancellationToken = default);
}

public interface ITwilioSmsProvider : ISmsProvider { }
public interface IAwsSnsSmsProvider : ISmsProvider { }
