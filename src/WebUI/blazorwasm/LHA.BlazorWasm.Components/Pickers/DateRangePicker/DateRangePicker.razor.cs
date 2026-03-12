using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.DateRangePicker;

/// <summary>
/// Orchestrates dual instances of the DatePicker pipeline resolving Start/End arrays logically globally safely.
/// </summary>
public partial class DateRangePicker : PickerBase<DateRange?>
{
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string ConfirmText { get; set; } = "Apply";

    // Split views for independent but interconnected months tracking
    protected PickerState LeftState { get; } = new();
    protected PickerState RightState { get; } = new();

    private DateRange? _tempValue;
    private int _clickCount = 0; // Tracks if we are pushing to start or end bounds algorithmically natively

    protected override void OnInitialized()
    {
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
            
            if (_tempValue?.Start.HasValue == true)
            {
                var start = _tempValue.Value.Start.Value;
                LeftState.CurrentMonth = new DateTime(start.Year, start.Month, 1);
                
                var end = _tempValue.Value.End;
                if (end.HasValue && (end.Value.Month != start.Month || end.Value.Year != start.Year))
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
            if (!Value.HasValue || !Value.Value.Start.HasValue) return string.Empty;
            
            var start = Value.Value.Start.Value.ToString(Format);
            var end = Value.Value.End.HasValue ? Value.Value.End.Value.ToString(Format) : "";
            
            return $"{start}{Separator}{end}";
        }
    }

    private Task HandleDateSelected(DateTime date)
    {
        if (Min.HasValue && date < Min.Value.Date) return Task.CompletedTask;
        if (Max.HasValue && date > Max.Value.Date) return Task.CompletedTask;

        if (_clickCount == 0 || (_tempValue?.IsComplete == true))
        {
            // Reset bounds entirely restarting workflow mathematically tracking sequentially forward securely
            _tempValue = new DateRange(date, null);
            _clickCount = 1;
        }
        else if (_clickCount == 1)
        {
            // Second click logic binding - enforces strictly chronological assignments natively safely without sorting
            var start = _tempValue!.Value.Start!.Value;
            if (date < start)
            {
                 // Swapped visually gracefully mapping logic implicitly directly behind-the-scenes
                _tempValue = new DateRange(date, start);
            }
            else
            {
                _tempValue = new DateRange(start, date);
            }
            _clickCount = 0;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ConfirmSelection()
    {
        if (_tempValue?.IsComplete == true)
        {
            await UpdateValueAsync(_tempValue);
        }
        ClosePopup();
    }
    
    protected override Task ClearAsync()
    {
        _tempValue = null;
        _clickCount = 0;
        return base.ClearAsync();
    }
}
