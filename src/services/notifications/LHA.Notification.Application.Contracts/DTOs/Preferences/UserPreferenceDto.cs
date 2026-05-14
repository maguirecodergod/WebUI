namespace LHA.Notification.Application.Contracts;

public record UserPreferenceDto(
    Guid Id,
    Guid TenantId,
    Guid UserId,
    bool GlobalOptOut,
    List<ChannelPreferenceDto> Channels,
    List<CategoryPreferenceDto> Categories,
    QuietHoursDto? QuietHours,
    string Timezone,
    string Locale,
    DateTimeOffset UpdatedAt);
