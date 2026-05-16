using System.Net.Http.Headers;
using System.Text;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure.Channels.Sms.Twilio;

public sealed class TwilioSmsProvider : BaseSmsProvider, ITwilioSmsProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TwilioSmsProvider> _logger;

    public TwilioSmsProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        ILogger<TwilioSmsProvider> logger) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.Sms;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(recipientId) && recipientId.StartsWith('+'));
    }

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToTwilioSettings();

        if (settings == null || string.IsNullOrEmpty(settings.AccountSid) || string.IsNullOrEmpty(settings.AuthToken))
        {
            _logger.LogWarning("Twilio not configured for tenant {TenantId}. AccountSid or AuthToken is missing.", tenantId);
            return;
        }

        HttpClient client = _httpClientFactory.CreateClient("twilio");
        string url = settings.Endpoint ?? $"https://api.twilio.com/2010-04-01/Accounts/{settings.AccountSid}/Messages.json";

        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.AccountSid}:{settings.AuthToken}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["To"] = recipientId,
            ["From"] = settings.FromPhoneNumber,
            ["Body"] = body
        });

        HttpResponseMessage response = await client.PostAsync(url, formData, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Twilio send failed for tenant {TenantId}: {StatusCode} {Error}", tenantId, response.StatusCode, error);
            throw new InvalidOperationException($"Twilio send failed: {response.StatusCode}");
        }

        _logger.LogInformation("Twilio SMS sent to {Recipient} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToTwilioSettings();
        return !string.IsNullOrWhiteSpace(settings?.AccountSid);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }

    public override async Task<bool> SendSmsAsync(string recipientPhone, Guid tenantId, string message, CancellationToken cancellationToken = default)
    {
        await SendAsync(recipientPhone, tenantId, "", message, cancellationToken: cancellationToken);
        return true;
    }
}
