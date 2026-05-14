using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain;

public sealed class TenantChannelConfigurationEntity : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; private set; }
    public CNotificationChannel Channel { get; private set; }
    public CProviderType ProviderType { get; private set; }
    public string? ServiceAccountJson { get; private set; }
    public string? ApiKey { get; private set; }
    public string? Host { get; private set; }
    public int? Port { get; private set; }
    public string? Username { get; private set; }
    public string? Password { get; private set; }
    public bool UseSsl { get; private set; }
    public Dictionary<string, string>? CustomSettings { get; private set; }
    public bool IsEnabled { get; private set; }

    private TenantChannelConfigurationEntity() { }

    public TenantChannelConfigurationEntity(
        Guid? tenantId,
        CNotificationChannel channel,
        CProviderType providerType,
        string? serviceAccountJson = null,
        string? apiKey = null,
        string? host = null,
        int? port = null,
        string? username = null,
        string? password = null,
        bool useSsl = true,
        Dictionary<string, string>? customSettings = null)
    {
        TenantId = tenantId;
        Channel = channel;
        ProviderType = providerType;
        ServiceAccountJson = serviceAccountJson;
        ApiKey = apiKey;
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        UseSsl = useSsl;
        CustomSettings = customSettings;
        IsEnabled = true;
    }

    public void Update(
        CProviderType providerType,
        string? serviceAccountJson = null,
        string? apiKey = null,
        string? host = null,
        int? port = null,
        string? username = null,
        string? password = null,
        bool useSsl = true,
        Dictionary<string, string>? customSettings = null)
    {
        ProviderType = providerType;
        ServiceAccountJson = serviceAccountJson;
        ApiKey = apiKey;
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        UseSsl = useSsl;
        CustomSettings = customSettings;
    }

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
