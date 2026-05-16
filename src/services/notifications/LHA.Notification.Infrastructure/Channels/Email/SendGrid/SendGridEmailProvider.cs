using System.Net.Http.Headers;
using System.Net.Http.Json;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;

using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure.Channels.Email.SendGrid;

internal sealed class SendGridEmailProvider : BaseEmailProvider, ISendGridEmailProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SendGridRequestBuilder _requestBuilder;
    private readonly ILogger<SendGridEmailProvider> _logger;

    public SendGridEmailProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        SendGridRequestBuilder requestBuilder,
        ILogger<SendGridEmailProvider> logger) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _requestBuilder = requestBuilder;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.Email;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(recipientId.Contains('@'));
    }

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);

        var settings = config.ToSendGridSettings();
        if (string.IsNullOrEmpty(settings?.ApiKey))
        {
            _logger.LogWarning("SendGrid not configured for tenant {TenantId}. ApiKey is missing.", tenantId);
            return;
        }

        HttpClient client = _httpClientFactory.CreateClient("sendgrid");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        var endpoint = settings.Endpoint ?? "https://api.sendgrid.com/v3/mail/send";

        string? htmlBody = metadata?.GetValueOrDefault("htmlBody");
        string? notificationId = metadata?.GetValueOrDefault("notification_id");
        var request = _requestBuilder.BuildRequest(recipientId, subject, body, htmlBody, notificationId, settings);

        HttpResponseMessage response = await client.PostAsJsonAsync(endpoint, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("SendGrid send failed for tenant {TenantId}: {StatusCode} {Error}", tenantId, response.StatusCode, error);
            throw new InvalidOperationException($"SendGrid send failed: {response.StatusCode}");
        }

        _logger.LogInformation("SendGrid email sent to {Recipient} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> SendBatchAsync(IEnumerable<string> recipientIds, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);

        var settings = config.ToSendGridSettings();
        if (string.IsNullOrEmpty(settings?.ApiKey)) return false;

        HttpClient client = _httpClientFactory.CreateClient("sendgrid");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        var endpoint = settings.Endpoint ?? "https://api.sendgrid.com/v3/mail/send";

        string? htmlBody = metadata?.GetValueOrDefault("htmlBody");
        string? notificationId = metadata?.GetValueOrDefault("notification_id");
        var request = _requestBuilder.BuildBatchRequest(recipientIds, subject, body, htmlBody, notificationId, settings);

        HttpResponseMessage response = await client.PostAsJsonAsync(endpoint, request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToSendGridSettings();
        return !string.IsNullOrWhiteSpace(settings?.ApiKey);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }
}
