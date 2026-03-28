using System;

namespace LHA.Ddd.Domain;

/// <summary>
/// Base class for all filters, including auditing date ranges.
/// </summary>
public class BaseFilter
{
    public RangeFilterProperty<DateTimeOffset> CreatedDateRange { get; set; } = new RangeFilterProperty<DateTimeOffset>();
    public RangeFilterProperty<DateTimeOffset> ModifiedDateRange { get; set; } = new RangeFilterProperty<DateTimeOffset>();
}

/// <summary>
/// Range-based filter for value types.
/// </summary>
/// <typeparam name="T">The underlying type.</typeparam>
public class RangeFilterProperty<T> where T : struct
{
    /// <summary>Starting value of the range (inclusive).</summary>
    public T? Start { get; set; }
    
    /// <summary>Ending value of the range (inclusive).</summary>
    public T? End { get; set; }
}
