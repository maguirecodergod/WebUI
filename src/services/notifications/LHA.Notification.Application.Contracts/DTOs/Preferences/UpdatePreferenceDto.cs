namespace LHA.Notification.Application.Contracts;

public record UpdatePreferenceDto(
    bool? GlobalOptOut,
    List<ChannelPreferenceDto>? Channels,
    List<CategoryPreferenceDto>? Categories,
    QuietHoursDto? QuietHours,
    string? Timezone,
    string? Locale);
