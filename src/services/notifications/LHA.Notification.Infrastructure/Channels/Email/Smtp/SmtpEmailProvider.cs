using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using LHA.Notification.Application.Contracts;

namespace LHA.Notification.Infrastructure.Channels.Email.Smtp;

internal sealed class SmtpEmailProvider : BaseEmailProvider, ISmtpEmailProvider
{
    private readonly ILogger<SmtpEmailProvider> _logger;

    public SmtpEmailProvider(
        IChannelConfigurationStore configStore,
        ILogger<SmtpEmailProvider> logger) : base(configStore)
    {
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
        var settings = config.ToSmtpSettings();

        if (settings == null || string.IsNullOrEmpty(settings.Host))
        {
            _logger.LogWarning("SMTP not configured for tenant {TenantId}. Host is missing.", tenantId);
            return;
        }

        var message = new MimeMessage();
        var fromEmail = settings.FromEmail ?? "noreply@example.com";
        var fromName = settings.FromName ?? "Notification Service";

        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(recipientId));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        if (metadata?.ContainsKey("htmlBody") == true)
        {
            bodyBuilder.HtmlBody = metadata["htmlBody"];
        }
        else
        {
            bodyBuilder.TextBody = body;
        }
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(settings.Host, settings.Port, settings.UseSsl, cancellationToken);

            if (!string.IsNullOrEmpty(settings.Username))
            {
                await client.AuthenticateAsync(settings.Username, settings.Password ?? string.Empty, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("SMTP email sent to {Recipient} via {Host} for tenant {TenantId}", recipientId, settings.Host, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP send failed for tenant {TenantId} via {Host}", tenantId, settings.Host);
            throw;
        }
    }

    public override async Task<bool> IsAvailableAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var config = await GetConfigurationAsync(tenantId, cancellationToken);
        var settings = config.ToSmtpSettings();
        return !string.IsNullOrWhiteSpace(settings?.Host);
    }
}
