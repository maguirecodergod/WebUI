using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class LinkDialog : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<LinkDialogResult?> OnSubmit { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private string Url { get; set; } = "";
    private string DisplayText { get; set; } = "";
    private string Title { get; set; } = "";
    private bool OpenInNewTab { get; set; } = true;

    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(Url)) return;

        var result = new LinkDialogResult
        {
            Url = Url,
            DisplayText = string.IsNullOrWhiteSpace(DisplayText) ? Url : DisplayText,
            Title = Title,
            OpenInNewTab = OpenInNewTab
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
        Url = "";
        DisplayText = "";
        Title = "";
        OpenInNewTab = true;
    }
}

public class LinkDialogResult
{
    public string Url { get; set; } = "";
    public string DisplayText { get; set; } = "";
    public string Title { get; set; } = "";
    public bool OpenInNewTab { get; set; }
}
