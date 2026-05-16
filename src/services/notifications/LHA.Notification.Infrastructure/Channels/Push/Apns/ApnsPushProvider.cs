using System.Net.Http.Headers;
using System.Text;
using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure.Channels.Push.Apns;

public sealed class ApnsPushProvider : BasePushProvider, IApnsPushProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApnsJwtTokenProvider _tokenProvider;
    private readonly ApnsMessageBuilder _messageBuilder;
    private readonly ILogger<ApnsPushProvider> _logger;

    public ApnsPushProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        ApnsJwtTokenProvider tokenProvider,
        ApnsMessageBuilder messageBuilder,
        ILogger<ApnsPushProvider> logger) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _tokenProvider = tokenProvider;
        _messageBuilder = messageBuilder;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.Push;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(recipientId));

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToApnsSettings();

        if (settings == null || string.IsNullOrEmpty(settings.BundleId))
        {
            _logger.LogWarning("APNs not configured for tenant {TenantId}. BundleId is missing.", tenantId);
            return;
        }

        string host = settings.Endpoint ?? (settings.IsProduction ? "api.push.apple.com" : "api.sandbox.push.apple.com");
        var (payload, path) = _messageBuilder.Build(recipientId, subject, body, settings.BundleId, metadata);

        HttpClient client = _httpClientFactory.CreateClient("apns");
        client.BaseAddress = new Uri($"https://{host}");

        var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _tokenProvider.GetToken());
        request.Headers.TryAddWithoutValidation("apns-topic", settings.BundleId);
        request.Headers.TryAddWithoutValidation("apns-push-type", "alert");
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("APNs send failed for tenant {TenantId}: {StatusCode} {Error}", tenantId, response.StatusCode, error);
            throw new InvalidOperationException($"APNs send failed: {response.StatusCode}");
        }

        _logger.LogInformation("APNs message sent to {DeviceToken} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToApnsSettings();
        return !string.IsNullOrWhiteSpace(settings?.BundleId);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }
}
