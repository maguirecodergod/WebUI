using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class ColorPickerPopup : LHAComponentBase
{
    [Parameter] public string SelectedColor { get; set; } = "#000000";
    [Parameter] public EventCallback<string> OnColorSelected { get; set; }

    private static readonly string[] PresetColors =
    [
        // Row 1 - Basic colors
        "#000000", "#434343", "#666666", "#999999", "#b7b7b7", "#cccccc", "#d9d9d9", "#efefef", "#f3f3f3", "#ffffff",
        // Row 2 - Reds/Pinks
        "#980000", "#ff0000", "#ff9900", "#ffff00", "#00ff00", "#00ffff", "#4a86e8", "#0000ff", "#9900ff", "#ff00ff",
        // Row 3 - Muted/Dark tones
        "#e6b8af", "#f4cccc", "#fce5cd", "#fff2cc", "#d9ead3", "#d0e0e3", "#c9daf8", "#cfe2f3", "#d9d2e9", "#ead1dc",
        // Row 4 - Medium tones
        "#dd7e6b", "#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#a4c2f4", "#9fc5e8", "#b4a7d6", "#d5a6bd",
        // Row 5 - Saturated
        "#cc4125", "#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6d9eeb", "#6fa8dc", "#8e7cc3", "#c27ba0",
        // Row 6 - Dark saturated
        "#a61c00", "#cc0000", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3c78d8", "#3d85c6", "#674ea7", "#a64d79",
        // Row 7 - Darkest
        "#85200c", "#990000", "#b45f06", "#bf9000", "#38761d", "#134f5c", "#1155cc", "#0b5394", "#351c75", "#741b47"
    ];

    private async Task SelectColor(string color)
    {
        SelectedColor = color;
        if (OnColorSelected.HasDelegate)
        {
            await OnColorSelected.InvokeAsync(color);
        }
    }

    private async Task OnCustomColorChange(ChangeEventArgs e)
    {
        var color = e.Value?.ToString() ?? "#000000";
        await SelectColor(color);
    }
}
