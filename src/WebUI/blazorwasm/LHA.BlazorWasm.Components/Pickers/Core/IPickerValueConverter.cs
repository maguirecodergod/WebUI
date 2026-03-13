using System;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// Provides an abstraction for mapping between internal DateTime math and external binding types (DateTime, DateTimeOffset).
/// </summary>
public interface IPickerValueConverter<TValue>
{
    TValue FromDateTime(DateTime? dt);
    DateTime? ToDateTime(TValue? value);
}

public class DefaultPickerConverter<TValue> : IPickerValueConverter<TValue>
{
    public TValue FromDateTime(DateTime? dt)
    {
        if (dt == null) return default!;

        if (typeof(TValue) == typeof(DateTimeOffset?))
        {
            return (TValue)(object)(DateTimeOffset?)dt.Value;
        }
        if (typeof(TValue) == typeof(DateTimeOffset))
        {
            return (TValue)(object)(DateTimeOffset)dt.Value;
        }
        if (typeof(TValue) == typeof(DateTime))
        {
            return (TValue)(object)dt.Value;
        }
        
        return (TValue)(object)dt;
    }

    public DateTime? ToDateTime(TValue? value)
    {
        if (value == null) return null;

        if (value is DateTimeOffset dto) return dto.DateTime;
        if (value is DateTime dt) return dt;

        return value as DateTime?;
    }
}

public class DateRangeConverter<TInner> : IPickerValueConverter<DateRange<TInner>>
{
    private readonly IPickerValueConverter<TInner> _innerConverter = new DefaultPickerConverter<TInner>();

    public DateRange<TInner> FromDateTime(DateTime? dt)
    {
        return new DateRange<TInner>(_innerConverter.FromDateTime(dt), default!);
    }

    public DateTime? ToDateTime(DateRange<TInner> value)
    {
        return _innerConverter.ToDateTime(value.Start);
    }
    
    public DateRange<TInner> CreateRange(DateTime? start, DateTime? end)
    {
        return new DateRange<TInner>(_innerConverter.FromDateTime(start), _innerConverter.FromDateTime(end));
    }
    
    public (DateTime? Start, DateTime? End) MapRange(DateRange<TInner> value)
    {
        return (_innerConverter.ToDateTime(value.Start), _innerConverter.ToDateTime(value.End));
    }
}
