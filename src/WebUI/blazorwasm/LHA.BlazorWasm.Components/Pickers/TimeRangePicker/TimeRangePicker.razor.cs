using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.TimeRangePicker;

/// <summary>
/// Dual clock instantiation pushing structural variables cleanly mapping generic base bounds natively over logical ranges sequentially.
/// </summary>
public partial class TimeRangePicker : PickerBase<DateRange?>
{
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string ConfirmText { get; set; } = "Apply";
    [Parameter] public bool Is24Hour { get; set; } = false;

    protected PickerState StartState { get; } = new();
    protected PickerState EndState { get; } = new();

    private DateRange? _tempValue;

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Format))
        {
            Format = Is24Hour ? "HH:mm" : "hh:mm tt";
        }
    }

    protected override void OnParametersSet()
    {
        if (Value.HasValue && !State.IsOpen)
        {
            _tempValue = Value;
            SyncStateWithInternalValue();
        }
    }
    
    private void SyncStateWithInternalValue()
    {
        if (_tempValue?.Start.HasValue == true)
        {
            SetStateFromTime(StartState, _tempValue.Value.Start.Value);
        }
        else
        {
            // Default 9:00 AM
            SetStateFromTime(StartState, DateTime.Today.AddHours(9));
        }
        
        if (_tempValue?.End.HasValue == true)
        {
            SetStateFromTime(EndState, _tempValue.Value.End.Value);
        }
        else
        {
            // Default 5:00 PM
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
            if (!Value.HasValue || !Value.Value.Start.HasValue || !Value.Value.End.HasValue) return string.Empty;
            return $"{Value.Value.Start.Value.ToString(Format)}{Separator}{Value.Value.End.Value.ToString(Format)}";
        }
    }

    private Task HandleTimeChanged()
    {
        // Interacts seamlessly whenever any internal sub-select element broadcasts.
        // Syncs local variables strictly preserving logical bounds for visual accuracy
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
        // Project onto today
        return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, h, state.SelectedMinute, 0);
    }

    private async Task ConfirmSelection()
    {
        var start = GetTimeFromState(StartState);
        var end = GetTimeFromState(EndState);
        
        // Auto-fix inverted bindings natively enforcing logical flow strictly
        if (start > end)
        {
            _tempValue = new DateRange(end, start);
            SyncStateWithInternalValue(); // Repair visual elements to match chronological order
        }
        else
        {
            _tempValue = new DateRange(start, end);
        }
        
        await UpdateValueAsync(_tempValue);
        ClosePopup();
    }
    
    protected override Task ClearAsync()
    {
        _tempValue = null;
        return base.ClearAsync();
    }
}
