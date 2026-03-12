using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;
using LHA.BlazorWasm.Shared.Constants.Formatters;
using LHA.BlazorWasm.Services.Localization;

namespace LHA.BlazorWasm.Components.Pickers.DateTimeRangePicker;

/// <summary>
/// Root orchestrator combining native ranges alongside time offsets exclusively avoiding JS.
/// This utilizes dedicated internal UI buttons strictly mapping Left/Right bounds targeting specific dates.
/// </summary>
public partial class DateTimeRangePicker : PickerBase<DateRange?>
{
    [Inject] protected ILocalizationService LocalizationService { get; set; } = default!;
    [Parameter] public string Separator { get; set; } = " ~ ";
    [Parameter] public string? ConfirmText { get; set; }
    private string ComputedConfirmText => ConfirmText ?? LocalizationService.L("DateTimeRangePicker.Apply");

    [Parameter] public bool Is24Hour { get; set; } = false;

    // Decoupled state mappings strictly ensuring left/right variables map without stepping on variables globally
    protected PickerState StartState { get; } = new();
    protected PickerState EndState { get; } = new();

    private DateRange? _tempValue;
    private int _clickCount = 0; // Only utilized when toggling exclusively via Calendar bounds sequentially

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
        if (_tempValue is { Start: DateTime start })
        {
            StartState.CurrentMonth = new DateTime(start.Year, start.Month, 1);
            SetTimeState(StartState, start);

            if (_tempValue.Value.End is DateTime end)
            {
                if (end.Month != start.Month || end.Year != start.Year)
                {
                    EndState.CurrentMonth = new DateTime(end.Year, end.Month, 1);
                }
                else
                {
                    EndState.CurrentMonth = StartState.CurrentMonth.AddMonths(1);
                }
                SetTimeState(EndState, end);
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
            if (Value is not { Start: DateTime start }) return string.Empty;

            var startStr = start.ToString(Format);
            var endStr = Value.Value.End is DateTime end ? end.ToString(Format) : string.Empty;

            return $"{startStr}{Separator}{endStr}";
        }
    }

    private Task HandleDateSelected(DateTime date, bool isStartPanel)
    {
        if (Min.HasValue && date < Min.Value.Date) return Task.CompletedTask;
        if (Max.HasValue && date > Max.Value.Date) return Task.CompletedTask;

        // Extract times natively before regenerating DateTime variables avoiding math truncations securely
        var startTime = GetTime(StartState);
        var endTime = GetTime(EndState);

        if (_clickCount == 0 || (_tempValue?.IsComplete == true))
        {
            _tempValue = new DateRange(
                new DateTime(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0),
                null);
            _clickCount = 1;
        }
        else if (_clickCount == 1 && _tempValue is { Start: DateTime existingStart })
        {
            var newBoundary = new DateTime(date.Year, date.Month, date.Day, endTime.Hour, endTime.Minute, 0);

            if (newBoundary < existingStart)
            {
                // Inverted click boundaries implicitly map gracefully globally strictly locally
                var tempEnd = new DateTime(existingStart.Year, existingStart.Month, existingStart.Day, endTime.Hour, endTime.Minute, 0);
                var tempStart = new DateTime(date.Year, date.Month, date.Day, startTime.Hour, startTime.Minute, 0);
                _tempValue = new DateRange(tempStart, tempEnd);
            }
            else
            {
                _tempValue = new DateRange(existingStart, newBoundary);
            }
            _clickCount = 0;
        }

        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleTimeChanged()
    {
        // Dynamically update logical limits matching current DOM boundaries actively during selection organically
        if (_tempValue is { Start: DateTime sd })
        {
            var st = GetTime(StartState);
            var newStart = new DateTime(sd.Year, sd.Month, sd.Day, st.Hour, st.Minute, 0);

            DateTime? newEnd = null;
            if (_tempValue.Value.End is DateTime ed)
            {
                var et = GetTime(EndState);
                newEnd = new DateTime(ed.Year, ed.Month, ed.Day, et.Hour, et.Minute, 0);
            }

            _tempValue = new DateRange(newStart, newEnd);
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
        return new DateTime(2000, 1, 1, h, state.SelectedMinute, 0); // Date component ignored implicitly
    }

    private async Task ConfirmSelection()
    {
        if (_tempValue is { Start: DateTime s, End: DateTime e })
        {
            if (s > e)
            {
                _tempValue = new DateRange(e, s);
                SyncStateWithInternalValue();
            }

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
