using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.TimePicker;

/// <summary>
/// Mapped natively rendering standalone component pushing discrete structural logic resolving `TimeSpan` internally
/// while pushing back standard exact output `DateTime` bindings.
/// </summary>
public partial class TimePicker<TValue> : PickerBase<TValue>
{
    [Parameter] public bool Is24Hour { get; set; } = false;

    // Default formatting strictly enforcing Time
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
        var dt = EffectiveConverter.ToDateTime(Value);
        if (dt.HasValue && !State.IsOpen)
        {
            var h = dt.Value.Hour;
            
            if (Is24Hour)
            {
                State.SelectedHour = h;
            }
            else
            {
                State.AmPm = h >= 12 ? "PM" : "AM";
                State.SelectedHour = h % 12;
                if (State.SelectedHour == 0) State.SelectedHour = 12; // 12 AM math edge case
            }
            
            State.SelectedMinute = dt.Value.Minute;
        }
    }

    private string FormattedValue 
    {
        get 
        {
            var dt = EffectiveConverter.ToDateTime(Value);
            return dt?.ToString(Format) ?? string.Empty;
        }
    }

    private async Task HandleTimeChanged()
    {
        var h = State.SelectedHour;
        if (!Is24Hour)
        {
            if (State.AmPm == "PM" && h < 12) h += 12;
            if (State.AmPm == "AM" && h == 12) h = 0;
        }

        // Apply time strictly bounding against `Now` baseline standard cleanly
        var dt = EffectiveConverter.ToDateTime(Value);
        var baseDate = dt ?? DateTime.Today;
        var newTime = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, h, State.SelectedMinute, 0);
        
        var newValue = EffectiveConverter.FromDateTime(newTime);
        await UpdateValueAsync(newValue);
    }
}
