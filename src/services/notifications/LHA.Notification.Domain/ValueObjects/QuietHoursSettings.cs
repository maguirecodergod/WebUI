namespace LHA.Notification.Domain.ValueObjects;

public sealed class QuietHoursSettings
{
    public bool Enabled { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string Timezone { get; private set; } = default!;

    public QuietHoursSettings(
        bool enabled,
        TimeOnly startTime,
        TimeOnly endTime,
        string timezone)
    {
        Enabled = enabled;
        StartTime = startTime;
        EndTime = endTime;
        Timezone = timezone;
    }

    public bool IsQuietHour(TimeOnly time, string timezone)
    {
        if (!Enabled)
            return false;

        if (StartTime < EndTime)
        {
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            return time >= StartTime || time <= EndTime;
        }
    }
}