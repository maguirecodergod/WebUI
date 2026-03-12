using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.DatePicker;

/// <summary>
/// A standardized standard DatePicker binding exclusively mapped to standard DateTime targets visually.
/// </summary>
public partial class DatePicker : PickerBase<DateTime?>
{
    protected override void OnParametersSet()
    {
        if (Value.HasValue && !State.IsOpen)
        {
            State.CurrentMonth = new DateTime(Value.Value.Year, Value.Value.Month, 1);
        }
    }

    private string FormattedValue => Value?.ToString(Format) ?? string.Empty;

    private async Task HandleDateSelected(DateTime date)
    {
        if (Min.HasValue && date < Min.Value.Date) return;
        if (Max.HasValue && date > Max.Value.Date) return;

        // Strip off any time variables natively strictly to date boundary
        await UpdateValueAsync(date.Date);
        ClosePopup();
    }
}
