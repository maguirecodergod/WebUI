using System.Text.Json;
using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Shared.Domain.Enums.Notification;
using LHA.Shared.Domain.Notification;

namespace LHA.Notification.Domain;

[Audited]
public sealed class ChannelConfigurationEntity : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public CNotificationChannel Channel { get; private set; }
    public CProviderType ProviderType { get; private set; }
    public string? SettingsJson { get; private set; }
    public bool IsEnabled { get; private set; }

    private ChannelConfigurationEntity() { }

    public ChannelConfigurationEntity(
        Guid? tenantId,
        CNotificationChannel channel,
        CProviderType providerType,
        ProviderSettings? settings = null)
    {
        TenantId = tenantId;
        Channel = channel;
        ProviderType = providerType;
        SettingsJson = settings != null ? JsonSerializer.Serialize(settings) : null;
        IsEnabled = true;
    }

    public void Update(CProviderType providerType, ProviderSettings? settings = null)
    {
        ProviderType = providerType;
        SettingsJson = settings != null ? JsonSerializer.Serialize(settings) : null;
    }

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public T? GetSettings<T>() where T : ProviderSettings
    {
        if (string.IsNullOrEmpty(SettingsJson)) return null;
        try
        {
            return JsonSerializer.Deserialize<T>(SettingsJson);
        }
        catch
        {
            return null;
        }
    }
}
