using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;
using LHA.BlazorWasm.Shared.Constants.Formatters;
using LHA.BlazorWasm.Services.Localization;

namespace LHA.BlazorWasm.Components.Pickers.DateTimeRangePicker;

/// <summary>
/// Root orchestrator combining native ranges alongside time offsets exclusively avoiding JS.
/// This utilizes dedicated internal UI buttons strictly mapping Left/Right bounds targeting specific dates.
/// </summary>
public partial class DateTimeRangePicker<TInner> : PickerBase<DateRange<TInner>>
{
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string? ConfirmText { get; set; }
    private string ComputedConfirmText => ConfirmText ?? LocalizationService.L("Common.Apply");

    [Parameter] public bool Is24Hour { get; set; } = false;

    // Decoupled state mappings strictly ensuring left/right variables map without stepping on variables globally
    protected PickerState StartState { get; } = new();
    protected PickerState EndState { get; } = new();

    private DateRange<TInner> _tempValue;
    private int _clickCount = 0;

    private DateRangeConverter<TInner> RangeConverter => new();

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Format))
        {
            Format = Is24Hour ? DateTimeFormatter.DateTime24 : DateTimeFormatter.DateTime12;
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
            StartState.CurrentMonth = new DateTime(start.Value.Year, start.Value.Month, 1);
            SetTimeState(StartState, start.Value);

            if (end.HasValue)
            {
                if (end.Value.Month != start.Value.Month || end.Value.Year != start.Value.Year)
                {
                    EndState.CurrentMonth = new DateTime(end.Value.Year, end.Value.Month, 1);
                }
                else
                {
                    EndState.CurrentMonth = StartState.CurrentMonth.AddMonths(1);
                }
                SetTimeState(EndState, end.Value);
            }
            else
            {
                EndState.CurrentMonth = StartState.CurrentMonth.AddMonths(1);
                SetTimeState(EndState, DateTime.Today.AddHours(17));
            }
        }
        else
        {
            var now = DateTime.Now;
            StartState.CurrentMonth = new DateTime(now.Year, now.Month, 1);
            EndState.CurrentMonth = StartState.CurrentMonth.AddMonths(1);
            SetTimeState(StartState, DateTime.Today.AddHours(9));
            SetTimeState(EndState, DateTime.Today.AddHours(17));
        }
    }

    private void SetTimeState(PickerState state, DateTime time)
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
            if (!start.HasValue) return string.Empty;

            var startStr = start.Value.ToString(Format);
            var endStr = end.HasValue ? end.Value.ToString(Format) : string.Empty;

            return $"{startStr}{Separator}{endStr}";
        }
    }

    private Task HandleDateSelected(DateTime date, bool isStartPanel)
    {
        if (Min.HasValue && date < Min.Value.Date) return Task.CompletedTask;
        if (Max.HasValue && date > Max.Value.Date) return Task.CompletedTask;

        var startTime = GetTime(StartState);
        var endTime = GetTime(EndState);
        
        var (currentStart, currentEnd) = RangeConverter.MapRange(_tempValue);

        if (_clickCount == 0 || _tempValue.IsComplete)
        {
            _tempValue = RangeConverter.CreateRange(
                new DateTime(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0),
                null);
            _clickCount = 1;
        }
        else if (_clickCount == 1 && currentStart.HasValue)
        {
            var existingStart = currentStart.Value;
            var newBoundary = new DateTime(date.Year, date.Month, date.Day, endTime.Hour, endTime.Minute, 0);

            if (newBoundary < existingStart)
            {
                var tempEnd = new DateTime(existingStart.Year, existingStart.Month, existingStart.Day, endTime.Hour, endTime.Minute, 0);
                var tempStart = new DateTime(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0);
                _tempValue = RangeConverter.CreateRange(tempStart, tempEnd);
            }
            else
            {
                _tempValue = RangeConverter.CreateRange(existingStart, newBoundary);
            }
            _clickCount = 0;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleTimeChanged()
    {
        var (sd, ed) = RangeConverter.MapRange(_tempValue);
        if (sd.HasValue)
        {
            var st = GetTime(StartState);
            var newStart = new DateTime(sd.Value.Year, sd.Value.Month, sd.Value.Day, st.Hour, st.Minute, 0);

            DateTime? newEnd = null;
            if (ed.HasValue)
            {
                var et = GetTime(EndState);
                newEnd = new DateTime(ed.Value.Year, ed.Value.Month, ed.Value.Day, et.Hour, et.Minute, 0);
            }

            _tempValue = RangeConverter.CreateRange(newStart, newEnd);
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private DateTime GetTime(PickerState state)
    {
        var h = state.SelectedHour;
        if (!Is24Hour)
        {
            if (state.AmPm == "PM" && h < 12) h += 12;
            if (state.AmPm == "AM" && h == 12) h = 0;
        }
        return new DateTime(2000, 1, 1, h, state.SelectedMinute, 0);
    }

    private async Task ConfirmSelection()
    {
        var (s, e) = RangeConverter.MapRange(_tempValue);
        if (s.HasValue && e.HasValue)
        {
            if (s > e)
            {
                _tempValue = RangeConverter.CreateRange(e, s);
                SyncStateWithInternalValue();
            }

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
