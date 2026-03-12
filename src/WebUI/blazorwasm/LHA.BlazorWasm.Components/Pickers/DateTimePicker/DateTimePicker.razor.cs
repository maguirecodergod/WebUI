using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.DateTimePicker;

/// <summary>
/// A dense layout mapping combination states of both pure explicit date clicks mapping into dynamic hour/minute combinations securely.
/// </summary>
public partial class DateTimePicker : PickerBase<DateTime?>
{
    [Parameter] public bool Is24Hour { get; set; } = false;
    [Parameter] public string ConfirmText { get; set; } = "Apply";

    /// <summary>
    /// Tracks temporarily unsaved changes blocking DOM push until strictly confirmed.
    /// </summary>
    private DateTime? _internalValue;

    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Format))
        {
            Format = Is24Hour ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd hh:mm tt";
        }
    }

    protected override void OnParametersSet()
    {
        if (Value.HasValue && !State.IsOpen)
        {
            _internalValue = Value;
            SyncStateWithInternalValue();
        }
        else if (!_internalValue.HasValue && !Value.HasValue)
        {
            // Default explicit UI positions cleanly when no inputs provided globally
            _internalValue = DateTime.Today;
            SyncStateWithInternalValue();
        }
    }
    
    private void SyncStateWithInternalValue()
    {
        if (!_internalValue.HasValue) return;
        
        State.CurrentMonth = new DateTime(_internalValue.Value.Year, _internalValue.Value.Month, 1);
        
        var h = _internalValue.Value.Hour;
        if (Is24Hour)
        {
            State.SelectedHour = h;
        }
        else
        {
            State.AmPm = h >= 12 ? "PM" : "AM";
            State.SelectedHour = h % 12;
            if (State.SelectedHour == 0) State.SelectedHour = 12;
        }
        State.SelectedMinute = _internalValue.Value.Minute;
    }

    private string FormattedValue => Value?.ToString(Format) ?? string.Empty;

    private Task HandleDateSelected(DateTime date)
    {
        if (Min.HasValue && date < Min.Value.Date) return Task.CompletedTask;
        if (Max.HasValue && date > Max.Value.Date) return Task.CompletedTask;

        var time = _internalValue ?? DateTime.Now;
        _internalValue = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
        
        StateHasChanged();
        return Task.CompletedTask; // Selection is explicitly not pushed to Output Value until confirmation requested
    }
    
    private Task HandleTimeChanged()
    {
        var h = State.SelectedHour;
        if (!Is24Hour)
        {
            if (State.AmPm == "PM" && h < 12) h += 12;
            if (State.AmPm == "AM" && h == 12) h = 0;
        }

        var baseDate = _internalValue ?? DateTime.Today;
        _internalValue = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, h, State.SelectedMinute, 0);
        
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ConfirmSelection()
    {
        if (_internalValue.HasValue)
        {
            // Apply logic triggers strictly now modifying global properties mappings
            await UpdateValueAsync(_internalValue.Value);
        }
        ClosePopup();
    }
    
    protected override Task ClearAsync()
    {
         _internalValue = null;
         return base.ClearAsync();
    }
}
