using LHA.Notification.Application.Contracts;
using LHA.Notification.Domain.Repositories;
using LHA.Notification.Domain.Shared;
using Microsoft.Extensions.Logging;
using LHA.Notification.Infrastructure.Channels;

namespace LHA.Notification.Infrastructure.Channels.InApp;

public sealed class InAppChannelProvider : BaseProvider, IInAppProvider
{
    private readonly IUnreadCountCache _unreadCountCache;
    private readonly ILogger<InAppChannelProvider> _logger;

    public InAppChannelProvider(
        IChannelConfigurationStore configStore,
        IUnreadCountCache unreadCountCache,
        ILogger<InAppChannelProvider> logger) : base(configStore)
    {
        _unreadCountCache = unreadCountCache;
        _logger = logger;
    }

    public override CNotificationChannel Channel => CNotificationChannel.InApp;

    public override Task<bool> ValidateRecipientAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(!string.IsNullOrWhiteSpace(recipientId) && Guid.TryParse(recipientId, out _));

    public override async Task SendAsync(string recipientId, Guid tenantId, string subject, string body, Dictionary<string, string>? metadata = null, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(recipientId, out var userId))
        {
            await _unreadCountCache.IncrementUnreadCountAsync(tenantId, userId, cancellationToken);
        }
        _logger.LogInformation("InApp notification delivered to {RecipientId} for tenant {TenantId}", recipientId, tenantId);
    }

    public Task<bool> SendInAppAsync(string recipientId, Guid tenantId, string title, string body, string? actionUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<bool> SendTemplateInAppAsync(string recipientId, Guid tenantId, Guid templateId, Dictionary<string, object>? variables = null, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<bool> SendBulkInAppAsync(IEnumerable<string> recipientIds, Guid tenantId, string title, string body, string? actionUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<bool> DeleteInAppAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<bool> MarkAsReadAsync(Guid notificationId, string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public async Task<int> GetUnreadCountAsync(string recipientId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(recipientId, out var userId))
        {
            return await _unreadCountCache.GetUnreadCountAsync(tenantId, userId, cancellationToken);
        }
        return 0;
    }
}
