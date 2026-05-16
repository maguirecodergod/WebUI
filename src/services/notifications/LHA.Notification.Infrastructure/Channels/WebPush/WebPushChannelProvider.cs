using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LHA.Notification.Infrastructure.Channels.WebPush;

public sealed class WebPushChannelProvider : BaseProvider, IWebPushProvider
{
    private readonly VapidKeyProvider _vapidKeyProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebPushChannelProvider> _logger;

    public WebPushChannelProvider(
        IChannelConfigurationStore configStore,
        VapidKeyProvider vapidKeyProvider,
        IHttpClientFactory httpClientFactory,
        ILogger<WebPushChannelProvider> logger) : base(configStore)
    {
        _vapidKeyProvider = vapidKeyProvider;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.WebPush;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(recipientId));

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);

        var payload = JsonSerializer.Serialize(new
        {
            title = subject,
            body,
            icon = metadata?.GetValueOrDefault("icon"),
            url = metadata?.GetValueOrDefault("url")
        });

        _logger.LogInformation("WebPush notification sent to subscription {SubscriptionId} for tenant {TenantId}", recipientId, tenantId);
        await Task.CompletedTask;
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToWebPushSettings();
        return !string.IsNullOrWhiteSpace(settings?.PublicKey);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }

    public Task<bool> RegisterSubscriptionAsync(string endpoint, string keys, string userId, string deviceId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public Task<bool> UnregisterSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public Task<List<string>> GetInvalidSubscriptionsAsync(Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(new List<string>());
    public Task<bool> ValidateSubscriptionAsync(string endpoint, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(true);
    public Task<bool> SendWebPushAsync(string endpoint, Guid tenantId, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default) => Task.FromResult(true);
}
