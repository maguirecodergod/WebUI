using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using LHA.Notification.Domain.Shared;

namespace LHA.Notification.Domain;

public sealed class DeviceEntity : FullAuditedEntity<Guid>,
    IMultiTenant
{
    public Guid UserId { get; private set; } = default!;
    public CDevicePlatform Platform { get; private set; }
    public string Token { get; private set; } = default!;
    public string TokenHash { get; private set; } = default!;
    public string AppVersion { get; private set; } = default!;
    public string OsVersion { get; private set; } = default!;
    public string DeviceModel { get; private set; } = default!;
    public string Locale { get; private set; } = default!;
    public string Timezone { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime LastSeenAt { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public Guid? TenantId { get; private set; }


    private DeviceEntity()
    {
    }

    public DeviceEntity(
        Guid userId,
        CDevicePlatform platform,
        string token,
        string appVersion,
        string osVersion,
        string deviceModel,
        string locale,
        string timezone)
    {
        UserId = userId;
        Platform = platform;
        Token = token;
        TokenHash = HashToken(token);
        AppVersion = appVersion;
        OsVersion = osVersion;
        DeviceModel = deviceModel;
        Locale = locale;
        Timezone = timezone;
        IsActive = true;
        LastSeenAt = DateTime.UtcNow;
        RegisteredAt = DateTime.UtcNow;
    }

    private static string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public void UpdateLastSeen(DateTime lastSeenAt)
    {
        LastSeenAt = lastSeenAt;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
