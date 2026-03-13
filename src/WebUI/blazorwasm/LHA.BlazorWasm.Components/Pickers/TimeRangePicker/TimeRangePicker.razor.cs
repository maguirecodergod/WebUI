using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.TimeRangePicker;

/// <summary>
/// Dual clock instantiation pushing structural variables cleanly mapping generic base bounds natively over logical ranges sequentially.
/// </summary>
public partial class TimeRangePicker<TInner> : PickerBase<DateRange<TInner>>
{
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string? ConfirmText { get; set; }
    protected string ComputedConfirmText => ConfirmText ?? LocalizationService.L("Common.Apply");
    [Parameter] public bool Is24Hour { get; set; } = false;

    protected PickerState StartState { get; } = new();
    protected PickerState EndState { get; } = new();

    private DateRange<TInner> _tempValue;
    private DateRangeConverter<TInner> RangeConverter => new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (string.IsNullOrEmpty(Format))
        {
            Format = Is24Hour ? "HH:mm" : "hh:mm tt";
        }
    }

    protected override void OnParametersSet()
    {
        if (!State.IsOpen)
        {
            _tempValue = Value;
            SyncStateWithInternalValue();
        }
    }
    
    private void SyncStateWithInternalValue()
    {
        var (start, end) = RangeConverter.MapRange(_tempValue);
        if (start.HasValue)
        {
            SetStateFromTime(StartState, start.Value);
        }
        else
        {
            SetStateFromTime(StartState, DateTime.Today.AddHours(9));
        }
        
        if (end.HasValue)
        {
            SetStateFromTime(EndState, end.Value);
        }
        else
        {
             SetStateFromTime(EndState, DateTime.Today.AddHours(17));
        }
    }
    
    private void SetStateFromTime(PickerState state, DateTime time)
    {
        var h = time.Hour;
        if (Is24Hour)
        {
            state.SelectedHour = h;
        }
        else
        {
            state.AmPm = h >= 12 ? "PM" : "AM";
            state.SelectedHour = h % 12;
            if (state.SelectedHour == 0) state.SelectedHour = 12;
        }
        state.SelectedMinute = time.Minute;
    }

    private string FormattedValue
    {
        get
        {
            var (start, end) = RangeConverter.MapRange(Value);
            if (!start.HasValue || !end.HasValue) return string.Empty;
            return $"{start.Value.ToString(Format)}{Separator}{end.Value.ToString(Format)}";
        }
    }

    private Task HandleTimeChanged()
    {
        StateHasChanged();
        return Task.CompletedTask;
    }
    
    private DateTime GetTimeFromState(PickerState state)
    {
        var h = state.SelectedHour;
        if (!Is24Hour)
        {
            if (state.AmPm == "PM" && h < 12) h += 12;
            if (state.AmPm == "AM" && h == 12) h = 0;
        }
        return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, h, state.SelectedMinute, 0);
    }

    private async Task ConfirmSelection()
    {
        var start = GetTimeFromState(StartState);
        var end = GetTimeFromState(EndState);
        
        if (start > end)
        {
            _tempValue = RangeConverter.CreateRange(end, start);
            SyncStateWithInternalValue();
        }
        else
        {
            _tempValue = RangeConverter.CreateRange(start, end);
        }
        
        await UpdateValueAsync(_tempValue);
        ClosePopup();
    }
    
    protected override Task ClearAsync()
    {
        _tempValue = default;
        return base.ClearAsync();
    }
}
