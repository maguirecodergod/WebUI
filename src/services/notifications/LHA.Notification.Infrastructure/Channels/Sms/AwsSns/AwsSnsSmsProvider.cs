using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LHA.Notification.Infrastructure.Channels;

namespace LHA.Notification.Infrastructure.Channels.Sms.AwsSns;

public sealed class AwsSnsSmsProvider : BaseSmsProvider, IAwsSnsSmsProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AwsSnsSmsProvider> _logger;

    public AwsSnsSmsProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        ILogger<AwsSnsSmsProvider> logger) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.Sms;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(recipientId) && recipientId.StartsWith('+'));

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);

        var settings = config.ToAwsSnsSettings();
        var region = settings?.Region ?? "us-east-1";
        var accessKey = settings?.AccessKey;
        var secretKey = settings?.SecretKey;

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            _logger.LogWarning("AWS SNS not configured for tenant {TenantId}. AccessKey or SecretKey is missing.", tenantId);
            return;
        }

        HttpClient client = _httpClientFactory.CreateClient("awssns");
        string endpoint = $"https://sns.{region}.amazonaws.com";

        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Action"] = "Publish",
            ["PhoneNumber"] = recipientId,
            ["Message"] = body
        });

        HttpResponseMessage response = await client.PostAsync(endpoint, formData, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("AWS SNS send failed for tenant {TenantId}: {StatusCode} {Error}", tenantId, response.StatusCode, error);
            throw new InvalidOperationException($"AWS SNS send failed: {response.StatusCode}");
        }

        _logger.LogInformation("AWS SNS SMS sent to {Recipient} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToAwsSnsSettings();
        return !string.IsNullOrWhiteSpace(settings?.AccessKey);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }
}
