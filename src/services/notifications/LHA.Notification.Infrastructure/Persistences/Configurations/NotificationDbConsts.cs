namespace LHA.Notification.Infrastructure.Persistences.Configurations;

/// <summary>
/// Column length constants for Notification entity properties.
/// Used by <see cref="IEntityTypeConfiguration{TEntity}"/> classes.
/// </summary>
internal static class NotificationDbConsts
{
    // ─── General ────────────────────────────────────────────
    public const int MaxCodeLength          = 128;
    public const int MaxNameLength          = 256;
    public const int MaxDescriptionLength   = 1024;
    public const int MaxLocaleLength        = 10;
    public const int MaxTimezoneLength      = 64;
    public const int MaxTokenLength         = 1024;
    public const int MaxUrlLength           = 2048;
    public const int MaxIpLength            = 45;
    public const int MaxVersionLength       = 64;

    // ─── Notification ───────────────────────────────────────
    public const int MaxCorrelationIdLength = 128;
    public const int MaxSubjectLength       = 512;
    public const int MaxBodyLength          = 4096;
    public const int MaxFailureReasonLength = 1024;
    public const int MaxExternalIdLength    = 256;

    // ─── Device ─────────────────────────────────────────────
    public const int MaxDeviceModelLength   = 256;
    public const int MaxHashLength          = 128;

    // ─── Template ───────────────────────────────────────────
    public const int MaxTemplateLength      = 4096;

    // ─── Channel Configuration ──────────────────────────────
    public const int MaxHostLength          = 256;
    public const int MaxUsernameLength      = 128;
    public const int MaxServiceAccountLength = 4096;
    public const int MaxApiKeyLength        = 512;
}
