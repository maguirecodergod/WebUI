using System;

namespace LHA.BlazorWasm.Components.Pickers.Core;

/// <summary>
/// State container tracking internal variables for Picker menus and calendar navigation.
/// </summary>
public class PickerState
{
    // Extracted out so all components share the same rendering/navigation math
    
    // Toggle for the parent dropdown
    public bool IsOpen { get; set; }
    
    // Time navigation
    public DateTime CurrentMonth { get; set; } = DateTime.Today;
    
    // Time selection
    public int SelectedHour { get; set; } = 0;
    public int SelectedMinute { get; set; } = 0;
    public string AmPm { get; set; } = "AM"; // AM or PM

    public PickerState()
    {
        var now = DateTime.Now;
        CurrentMonth = new DateTime(now.Year, now.Month, 1);
    }
}
