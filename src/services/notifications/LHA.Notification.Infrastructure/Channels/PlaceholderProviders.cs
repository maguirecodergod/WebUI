using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Infrastructure.Channels;

public abstract class BaseProvider : INotificationChannelProvider
{
    protected readonly IChannelConfigurationStore ConfigStore;

    protected BaseProvider(IChannelConfigurationStore configStore)
    {
        ConfigStore = configStore;
    }

    public abstract CNotificationChannel Channel { get; }

    public virtual Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public abstract Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default);

    public virtual async Task<bool> SendBatchAsync(IEnumerable<string> recipientIds, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        bool allSuccess = true;
        foreach (var recipientId in recipientIds)
        {
            try
            {
                await SendAsync(recipientId, tenantId, subject, body, metadata, variables, cancellationToken);
            }
            catch
            {
                allSuccess = false;
            }
        }
        return allSuccess;
    }

    public virtual async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await ConfigStore.IsChannelEnabledAsync(tenantId, Channel, cancellationToken);
    }

    public virtual async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var enabled = await IsAvailableAsync(tenantId, cancellationToken);
        return enabled ? "Available" : "Disabled";
    }

    protected async Task<ChannelConfigurationDto> GetConfigurationAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await ConfigStore.GetConfigurationAsync(tenantId, Channel, cancellationToken);
    }
}

public abstract class BasePushProvider : BaseProvider, IPushProvider
{
    protected BasePushProvider(IChannelConfigurationStore configStore) : base(configStore) { }

    public override CNotificationChannel Channel => CNotificationChannel.Push;

    public virtual Task<bool> RegisterDeviceAsync(string deviceToken, string userId, string deviceId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> UnregisterDeviceAsync(string deviceToken, string userId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<List<string>> GetInvalidTokensAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(new List<string>());
    public virtual Task<bool> ValidateTokenAsync(string deviceToken, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
}

public abstract class BaseEmailProvider : BaseProvider, IEmailProvider
{
    protected BaseEmailProvider(IChannelConfigurationStore configStore) : base(configStore) { }

    public override CNotificationChannel Channel => CNotificationChannel.Email;

    public virtual Task<bool> SendEmailAsync(string recipientEmail, Guid tenantId, string subject, string body, bool isHtml, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendTemplateEmailAsync(string recipientEmail, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, string? locale = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendBulkEmailAsync(IEnumerable<string> recipientEmails, Guid tenantId, string subject, string body, bool isHtml, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
}

public abstract class BaseSmsProvider : BaseProvider, ISmsProvider
{
    protected BaseSmsProvider(IChannelConfigurationStore configStore) : base(configStore) { }

    public override CNotificationChannel Channel => CNotificationChannel.Sms;

    public virtual Task<bool> SendSmsAsync(string recipientPhone, Guid tenantId, string message, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendTemplateSmsAsync(string recipientPhone, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, string? locale = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendBulkSmsAsync(IEnumerable<string> recipientPhones, Guid tenantId, string message, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendVerificationCodeAsync(string recipientPhone, Guid tenantId, string verificationCode, string tenantName, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendOtpSmsAsync(string recipientPhone, Guid tenantId, string otp, string purpose, CancellationToken cancellationToken = default) => Task.FromResult(true);
}

public class WebPushProvider : BaseProvider, IWebPushProvider
{
    public WebPushProvider(IChannelConfigurationStore configStore) : base(configStore) { }

    public override CNotificationChannel Channel => CNotificationChannel.WebPush;

    public override Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public virtual Task<bool> RegisterSubscriptionAsync(string endpoint, string keys, string userId, string deviceId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> UnregisterSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<List<string>> GetInvalidSubscriptionsAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(new List<string>());
    public virtual Task<bool> ValidateSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendWebPushAsync(string endpoint, Guid tenantId, string title, string body, Dictionary<string, string>? data, CancellationToken cancellationToken = default) => Task.FromResult(true);
}

public class InAppProvider : BaseProvider, IInAppProvider
{
    public InAppProvider(IChannelConfigurationStore configStore) : base(configStore) { }

    public override CNotificationChannel Channel => CNotificationChannel.InApp;

    public override Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public virtual Task<bool> SendInAppAsync(string recipientId, Guid tenantId, string title, string body, string? actionUrl, Dictionary<string, string>? data, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendTemplateInAppAsync(string recipientId, Guid tenantId, Guid templateId, Dictionary<string, object>? variables, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> SendBulkInAppAsync(IEnumerable<string> recipientIds, Guid tenantId, string title, string body, string? actionUrl, Dictionary<string, string>? data, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> DeleteInAppAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<bool> MarkAsReadAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public virtual Task<int> GetUnreadCountAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(0);
}
