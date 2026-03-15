using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class ImageDialog : LhaComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<ImageDialogResult?> OnSubmit { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string ImageUrl { get; set; } = "";
    private string AltText { get; set; } = "";
    private string Width { get; set; } = "";
    private string Height { get; set; } = "";

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(ImageUrl)) return;

        var result = new ImageDialogResult
        {
            Url = ImageUrl,
            AltText = AltText,
            Width = Width,
            Height = Height
        };

        await OnSubmit.InvokeAsync(result);
        Reset();
    }

    private async Task Close()
    {
        Reset();
        await OnClose.InvokeAsync();
    }

    private void Reset()
    {
        ImageUrl = "";
        AltText = "";
        Width = "";
        Height = "";
    }
}

public class ImageDialogResult
{
    public string Url { get; set; } = "";
    public string AltText { get; set; } = "";
    public string Width { get; set; } = "";
    public string Height { get; set; } = "";
}
