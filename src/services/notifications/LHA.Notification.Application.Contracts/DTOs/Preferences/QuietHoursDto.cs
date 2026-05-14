namespace LHA.Notification.Application.Contracts;

public record QuietHoursDto(
    bool Enabled,
    string StartTime,
    string EndTime,
    string Timezone);