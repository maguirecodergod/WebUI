using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.DateRangePicker;

/// <summary>
/// Orchestrates dual instances of the DatePicker pipeline resolving Start/End arrays logically globally safely.
/// </summary>
public partial class DateRangePicker<TInner> : PickerBase<DateRange<TInner>>
{
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string? ConfirmText { get; set; }
    protected string ComputedConfirmText => ConfirmText ?? LocalizationService.L("Common.Apply");

    protected PickerState LeftState { get; } = new();
    protected PickerState RightState { get; } = new();

    private DateRange<TInner> _tempValue;
    private int _clickCount = 0;

    private DateRangeConverter<TInner> RangeConverter => new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Format))
        {
            Format = "yyyy-MM-dd";
        }
    }

    protected override void OnParametersSet()
    {
        if (!State.IsOpen)
        {
            _tempValue = Value;
            
            var (start, end) = RangeConverter.MapRange(_tempValue);
            if (start.HasValue)
            {
                LeftState.CurrentMonth = new DateTime(start.Value.Year, start.Value.Month, 1);
                
                if (end.HasValue && (end.Value.Month != start.Value.Month || end.Value.Year != start.Value.Year))
                {
                    RightState.CurrentMonth = new DateTime(end.Value.Year, end.Value.Month, 1);
                }
                else
                {
                    RightState.CurrentMonth = LeftState.CurrentMonth.AddMonths(1);
                }
            }
            else
            {
                RightState.CurrentMonth = LeftState.CurrentMonth.AddMonths(1);
            }
        }
    }

    private string FormattedValue
    {
        get
        {
            var (start, end) = RangeConverter.MapRange(Value);
            if (!start.HasValue) return string.Empty;
            
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var startStr = start.Value.ToString(Format, culture);
            var endStr = end.HasValue ? end.Value.ToString(Format, culture) : "";
            
            return $"{startStr}{Separator}{endStr}";
        }
    }

    private Task HandleDateSelected(DateTime date)
    {
        if (Min.HasValue && date < Min.Value.Date) return Task.CompletedTask;
        if (Max.HasValue && date > Max.Value.Date) return Task.CompletedTask;

        var (currentStart, currentEnd) = RangeConverter.MapRange(_tempValue);

        if (_clickCount == 0 || _tempValue.IsComplete)
        {
            _tempValue = RangeConverter.CreateRange(date, null);
            _clickCount = 1;
        }
        else if (_clickCount == 1)
        {
            var start = currentStart!.Value;
            if (date < start)
            {
                _tempValue = RangeConverter.CreateRange(date, start);
            }
            else
            {
                _tempValue = RangeConverter.CreateRange(start, date);
            }
            _clickCount = 0;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ConfirmSelection()
    {
        if (_tempValue.IsComplete)
        {
            await UpdateValueAsync(_tempValue);
        }
        ClosePopup();
    }
    
    protected override Task ClearAsync()
    {
        _tempValue = default;
        _clickCount = 0;
        return base.ClearAsync();
    }

    private DateRange ConvertToLegacyRange(DateRange<TInner> range)
    {
         var (s, e) = RangeConverter.MapRange(range);
         return new DateRange(s, e);
    }
}
