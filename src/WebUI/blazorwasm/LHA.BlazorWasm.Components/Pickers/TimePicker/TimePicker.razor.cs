using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.TimePicker;

/// <summary>
/// Mapped natively rendering standalone component pushing discrete structural logic resolving `TimeSpan` internally
/// while pushing back standard exact output `DateTime` bindings.
/// </summary>
public partial class TimePicker : PickerBase<DateTime?>
{
    [Parameter] public bool Is24Hour { get; set; } = false;

    // Default formatting strictly enforcing Time
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
            var h = Value.Value.Hour;
            
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
            
            State.SelectedMinute = Value.Value.Minute;
        }
    }

    private string FormattedValue => Value?.ToString(Format) ?? string.Empty;

    private async Task HandleTimeChanged()
    {
        var h = State.SelectedHour;
        if (!Is24Hour)
        {
            if (State.AmPm == "PM" && h < 12) h += 12;
            if (State.AmPm == "AM" && h == 12) h = 0;
        }

        // Apply time strictly bounding against `Now` baseline standard cleanly
        var baseDate = Value ?? DateTime.Today;
        var newTime = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, h, State.SelectedMinute, 0);
        
        await UpdateValueAsync(newTime);
    }
}
