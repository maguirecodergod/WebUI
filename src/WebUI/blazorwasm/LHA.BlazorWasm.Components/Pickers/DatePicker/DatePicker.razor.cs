using LHA.BlazorWasm.Components.Pickers.Core;

namespace LHA.BlazorWasm.Components.Pickers.DatePicker;

/// <summary>
/// A standardized standard DatePicker binding exclusively mapped to standard DateTime targets visually.
/// </summary>
public partial class DatePicker<TValue> : PickerBase<TValue>
{
    protected override void OnInitialized()
    {
        if (string.IsNullOrEmpty(Format))
        {
            Format = "yyyy-MM-dd";
        }
    }

    protected override void OnParametersSet()
    {
        var dt = EffectiveConverter.ToDateTime(Value);
        if (dt.HasValue && !State.IsOpen)
        {
            State.CurrentMonth = new DateTime(dt.Value.Year, dt.Value.Month, 1);
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

    private async Task HandleDateSelected(DateTime date)
    {
        if (Min.HasValue && date < Min.Value.Date) return;
        if (Max.HasValue && date > Max.Value.Date) return;

        // Strip off any time variables natively strictly to date boundary
        var newValue = EffectiveConverter.FromDateTime(date.Date);
        await UpdateValueAsync(newValue);
        ClosePopup();
    }
}
