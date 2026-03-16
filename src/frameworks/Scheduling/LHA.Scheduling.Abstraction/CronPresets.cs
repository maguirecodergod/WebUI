namespace LHA.Scheduling;

/// <summary>
/// Common cron expression presets for recurring jobs.
/// Uses standard 5-field cron syntax: minute hour day-of-month month day-of-week.
/// All times are evaluated in the time zone specified in <see cref="RecurringJobOptions.TimeZoneId"/>.
/// </summary>
public static class CronPresets
{
    // ─── Every N Minutes ──────────────────────────────────────

    /// <summary>Every minute: * * * * *</summary>
    public const string EveryMinute = "* * * * *";

    /// <summary>Every 5 minutes: */5 * * * *</summary>
    public const string Every5Minutes = "*/5 * * * *";

    /// <summary>Every 10 minutes: */10 * * * *</summary>
    public const string Every10Minutes = "*/10 * * * *";

    /// <summary>Every 15 minutes: */15 * * * *</summary>
    public const string Every15Minutes = "*/15 * * * *";

    /// <summary>Every 30 minutes: */30 * * * *</summary>
    public const string Every30Minutes = "*/30 * * * *";

    // ─── Hourly ───────────────────────────────────────────────

    /// <summary>Every hour at :00: 0 * * * *</summary>
    public const string Hourly = "0 * * * *";

    /// <summary>Every 2 hours at :00: 0 */2 * * *</summary>
    public const string Every2Hours = "0 */2 * * *";

    /// <summary>Every 4 hours at :00: 0 */4 * * *</summary>
    public const string Every4Hours = "0 */4 * * *";

    /// <summary>Every 6 hours at :00: 0 */6 * * *</summary>
    public const string Every6Hours = "0 */6 * * *";

    /// <summary>Every 12 hours at :00: 0 */12 * * *</summary>
    public const string Every12Hours = "0 */12 * * *";

    // ─── Daily ────────────────────────────────────────────────

    /// <summary>Daily at midnight: 0 0 * * *</summary>
    public const string Daily = "0 0 * * *";

    /// <summary>Daily at 1:00 AM: 0 1 * * *</summary>
    public const string DailyAt1Am = "0 1 * * *";

    /// <summary>Daily at 3:00 AM: 0 3 * * *</summary>
    public const string DailyAt3Am = "0 3 * * *";

    /// <summary>Daily at 6:00 AM: 0 6 * * *</summary>
    public const string DailyAt6Am = "0 6 * * *";

    /// <summary>Daily at noon: 0 12 * * *</summary>
    public const string DailyAtNoon = "0 12 * * *";

    // ─── Weekly ───────────────────────────────────────────────

    /// <summary>Weekly on Monday at midnight: 0 0 * * 1</summary>
    public const string WeeklyMonday = "0 0 * * 1";

    /// <summary>Weekly on Sunday at midnight: 0 0 * * 0</summary>
    public const string WeeklySunday = "0 0 * * 0";

    // ─── Monthly ──────────────────────────────────────────────

    /// <summary>Monthly on the 1st at midnight: 0 0 1 * *</summary>
    public const string MonthlyFirst = "0 0 1 * *";

    /// <summary>Monthly on the 15th at midnight: 0 0 15 * *</summary>
    public const string MonthlyFifteenth = "0 0 15 * *";

    // ─── Helper Methods ───────────────────────────────────────

    /// <summary>
    /// Creates a cron expression for a specific minute of every hour.
    /// </summary>
    /// <param name="minute">Minute (0-59).</param>
    public static string HourlyAt(int minute)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minute);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minute, 59);
        return $"{minute} * * * *";
    }

    /// <summary>
    /// Creates a cron expression for a specific hour and minute every day.
    /// </summary>
    /// <param name="hour">Hour (0-23).</param>
    /// <param name="minute">Minute (0-59).</param>
    public static string DailyAt(int hour, int minute = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(hour);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hour, 23);
        ArgumentOutOfRangeException.ThrowIfNegative(minute);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minute, 59);
        return $"{minute} {hour} * * *";
    }

    /// <summary>
    /// Creates a cron expression that runs every N minutes.
    /// </summary>
    /// <param name="interval">Interval in minutes (1-59).</param>
    public static string EveryNMinutes(int interval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(interval, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(interval, 59);
        return interval == 1 ? EveryMinute : $"*/{interval} * * * *";
    }
}
