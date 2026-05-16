using Microsoft.Extensions.Logging;
using LHA.Notification.Application.Contracts;

namespace LHA.Notification.Infrastructure.Channels.Email.AwsSes;

public sealed class AwsSesEmailProvider : BaseEmailProvider, IAwsSesEmailProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AwsSesEmailProvider> _logger;

    public AwsSesEmailProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        ILogger<AwsSesEmailProvider> logger) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override async Task SendAsync(string recipientId,
        Guid tenantId,
        string subject,
        string body,
        Dictionary<string, string>? metadata = null,
        Dictionary<string, object>? variables = null,
        CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToAwsSesSettings();

        if (settings == null || string.IsNullOrEmpty(settings.AccessKey) || string.IsNullOrEmpty(settings.SecretKey))
        {
            _logger.LogWarning("AWS SES not configured for tenant {TenantId}. AccessKey or SecretKey is missing.", tenantId);
            return;
        }

        var fromEmail = settings.FromEmail ?? "noreply@example.com";
        var fromName = "Notification Service"; // Optional: could also be in settings

        HttpClient client = _httpClientFactory.CreateClient("awsses");
        string endpoint = settings.Endpoint ?? $"https://email.{settings.Region}.amazonaws.com";

        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Action"] = "SendEmail",
            ["Source"] = $"{fromName} <{fromEmail}>",
            ["Destination.ToAddresses.member.1"] = recipientId,
            ["Message.Subject.Data"] = subject,
            ["Message.Body.Text.Data"] = body,
            ["Message.Body.Html.Data"] = metadata?.GetValueOrDefault("htmlBody") ?? body
        });

        // Note: Real AWS SDK or manual signing would be needed here for production.
        // For this refactoring, we focus on the design pattern of dynamic config.
        HttpResponseMessage response = await client.PostAsync(endpoint, formData, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("AWS SES send failed for tenant {TenantId}: {StatusCode} {Error}", tenantId, response.StatusCode, error);
            throw new InvalidOperationException($"AWS SES send failed: {response.StatusCode}");
        }

        _logger.LogInformation("AWS SES email sent to {Recipient} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToAwsSesSettings();
        return !string.IsNullOrWhiteSpace(settings?.AccessKey) && !string.IsNullOrWhiteSpace(settings?.SecretKey);
    }
}
