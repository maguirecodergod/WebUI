using System;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Represents a range between two dates.
/// </summary>
public struct DateRange : IEquatable<DateRange>
{
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public DateRange(DateTime? start, DateTime? end)
    {
        Start = start;
        End = end;
    }

    public bool HasStart => Start.HasValue;
    public bool HasEnd => End.HasValue;
    public bool IsComplete => Start.HasValue && End.HasValue;

    public bool Contains(DateTime date)
    {
        if (!Start.HasValue || !End.HasValue) return false;
        var d = date.Date;
        return d >= Start.Value.Date && d <= End.Value.Date;
    }

    public override bool Equals(object? obj) => obj is DateRange range && Equals(range);

    public bool Equals(DateRange other)
    {
        return Start == other.Start && End == other.End;
    }

    public override int GetHashCode() => HashCode.Combine(Start, End);

    public static bool operator ==(DateRange left, DateRange right) => left.Equals(right);
    public static bool operator !=(DateRange left, DateRange right) => !(left == right);
}
