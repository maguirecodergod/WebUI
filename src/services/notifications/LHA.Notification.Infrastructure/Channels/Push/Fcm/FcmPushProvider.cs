using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Shared;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Infrastructure.Channels.Push.Fcm;

public sealed class FcmPushProvider : BasePushProvider, IFcmPushProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FcmPushProvider> _logger;
    private readonly FcmMessageBuilder _messageBuilder;
    private readonly FcmResponseMapper _responseMapper;

    public FcmPushProvider(
        IChannelConfigurationStore configStore,
        IHttpClientFactory httpClientFactory,
        ILogger<FcmPushProvider> logger,
        FcmMessageBuilder messageBuilder,
        FcmResponseMapper responseMapper) : base(configStore)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _messageBuilder = messageBuilder;
        _responseMapper = responseMapper;
    }

    public override CNotificationChannel Channel => CNotificationChannel.Push;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(recipientId));
    }

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToFcmSettings();

        if (settings == null || string.IsNullOrEmpty(settings.ProjectId))
        {
            _logger.LogWarning("FCM not configured for tenant {TenantId}. ProjectId is missing.", tenantId);
            return;
        }

        HttpClient client = _httpClientFactory.CreateClient("fcm");
        var message = _messageBuilder.BuildSingleMessage(recipientId, subject, body, metadata);
        string url = settings.Endpoint ?? $"https://fcm.googleapis.com/v1/projects/{settings.ProjectId}/messages:send";

        var response = await client.PostAsJsonAsync(url, new { message }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var mappedError = _responseMapper.MapError(errorContent);

            if (mappedError.IsTokenExpired)
            {
                _logger.LogWarning("FCM token expired for recipient {RecipientId} in tenant {TenantId}", recipientId, tenantId);
            }

            throw new InvalidOperationException($"FCM send failed: {mappedError.ErrorMessage}");
        }

        _logger.LogInformation("FCM message sent to {RecipientId} for tenant {TenantId}", recipientId, tenantId);
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToFcmSettings();
        return !string.IsNullOrWhiteSpace(settings?.ProjectId) || !string.IsNullOrWhiteSpace(settings?.ServiceAccountJson);
    }

    public override async Task<string> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var available = await IsAvailableAsync(tenantId, cancellationToken);
        return available ? "Available" : "Not Configured";
    }
}
