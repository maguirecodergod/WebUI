using System;
using System.Collections.Generic;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Represents a range between two values (usually DateTime or DateTimeOffset).
/// </summary>
public struct DateRange<TValue> : IEquatable<DateRange<TValue>>
{
    public TValue? Start { get; set; }
    public TValue? End { get; set; }

    public DateRange(TValue? start, TValue? end)
    {
        Start = start;
        End = end;
    }

    public bool HasStart => Start != null;
    public bool HasEnd => End != null;
    public bool IsComplete => Start != null && End != null;

    public override bool Equals(object? obj) => obj is DateRange<TValue> range && Equals(range);

    public bool Equals(DateRange<TValue> other)
    {
        return EqualityComparer<TValue>.Default.Equals(Start, other.Start) && 
               EqualityComparer<TValue>.Default.Equals(End, other.End);
    }

    public override int GetHashCode() => HashCode.Combine(Start, End);

    public static bool operator ==(DateRange<TValue> left, DateRange<TValue> right) => left.Equals(right);
    public static bool operator !=(DateRange<TValue> left, DateRange<TValue> right) => !(left == right);
}

/// <summary>
/// Legacy non-generic DateRange for backward compatibility or simple usage.
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

    public bool Equals(DateRange other) => Start == other.Start && End == other.End;
    public override bool Equals(object? obj) => obj is DateRange range && Equals(range);
    public override int GetHashCode() => HashCode.Combine(Start, End);
    public static bool operator ==(DateRange left, DateRange right) => left.Equals(right);
    public static bool operator !=(DateRange left, DateRange right) => !(left == right);
}
