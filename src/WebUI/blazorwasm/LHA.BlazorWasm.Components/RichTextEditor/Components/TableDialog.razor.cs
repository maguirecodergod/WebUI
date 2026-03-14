using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class TableDialog : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<(int rows, int cols)> OnSubmit { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private const int MaxRows = 10;
    private const int MaxCols = 10;
    private int HoverRows { get; set; } = 1;
    private int HoverCols { get; set; } = 1;

    private void OnCellHover(int row, int col)
    {
        HoverRows = row;
        HoverCols = col;
    }

    private async Task OnCellClick(int row, int col)
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync((row, col));
        }
        await Close();
    }

    private async Task Close()
    {
        HoverRows = 1;
        HoverCols = 1;
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
        }
    }
}
