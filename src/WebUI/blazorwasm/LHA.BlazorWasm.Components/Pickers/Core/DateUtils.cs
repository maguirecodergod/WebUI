namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Utility functions bridging native calendar math for C# Blazor rendering loops.
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// Generates exactly 6 weeks (42 days) padding previous and next months appropriately
    /// to build a complete classic Calendar Grid visually.
    /// </summary>
    public static List<DateTime> GenerateCalendarGrid(DateTime monthDate)
    {
        var firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

        // Find offset indicating Monday vs Sunday start
        var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        // Shift if treating Monday as start, usually Sunday (0) is default. We will use Sunday.

        var startOffset = firstDayOfWeek;

        var startDate = firstDayOfMonth.AddDays(-startOffset);
        var grid = new List<DateTime>(42); // 6 rows * 7 days

        for (int i = 0; i < 42; i++)
        {
            grid.Add(startDate.AddDays(i));
        }

        return grid;
    }

    public static bool IsSameDay(DateTime? date1, DateTime? date2)
    {
        if (!date1.HasValue || !date2.HasValue) return false;
        return date1.Value.Date == date2.Value.Date;
    }

    public static bool IsBetween(DateTime date, DateTime? start, DateTime? end)
    {
        if (!start.HasValue || !end.HasValue) return false;

        // Handle inverted ranges visually natively
        var s = start.Value.Date;
        var e = end.Value.Date;

        var min = s < e ? s : e;
        var max = s > e ? s : e;

        return date.Date >= min && date.Date <= max;
    }
}
