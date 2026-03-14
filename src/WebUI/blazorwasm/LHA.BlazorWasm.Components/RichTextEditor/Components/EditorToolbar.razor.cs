using LHA.BlazorWasm.Components.RichTextEditor.Models;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class EditorToolbar : ComponentBase
{
    [Parameter] public EditorState State { get; set; } = new();
    [Parameter] public EventCallback<(string command, string? value)> OnCommand { get; set; }
    [Parameter] public EventCallback OnToggleSourceView { get; set; }
    [Parameter] public EventCallback OnToggleFullscreen { get; set; }
    [Parameter] public EventCallback<LinkDialogResult> OnInsertLink { get; set; }
    [Parameter] public EventCallback<ImageDialogResult> OnInsertImage { get; set; }
    [Parameter] public EventCallback<(int rows, int cols)> OnInsertTable { get; set; }

    private ToolbarDropdown? _foreColorDropdown;
    private ToolbarDropdown? _backColorDropdown;

    private bool _showLinkDialog;
    private bool _showImageDialog;
    private bool _showTableDialog;

    private async Task ExecuteCommand(string command, string? value = null)
    {
        if (OnCommand.HasDelegate)
        {
            await OnCommand.InvokeAsync((command, value));
        }
    }

    private async Task OnForeColor(string color)
    {
        await ExecuteCommand("foreColor", color);
        _foreColorDropdown?.Close();
    }

    private async Task OnBackColor(string color)
    {
        await ExecuteCommand("backColor", color);
        _backColorDropdown?.Close();
    }

    private void ShowLinkDialog() => _showLinkDialog = true;
    private void ShowImageDialog() => _showImageDialog = true;
    private void ShowTableDialog() => _showTableDialog = true;

    private async Task OnLinkSubmit(LinkDialogResult? result)
    {
        _showLinkDialog = false;
        if (result != null && OnInsertLink.HasDelegate)
        {
            await OnInsertLink.InvokeAsync(result);
        }
    }

    private async Task OnImageSubmit(ImageDialogResult? result)
    {
        _showImageDialog = false;
        if (result != null && OnInsertImage.HasDelegate)
        {
            await OnInsertImage.InvokeAsync(result);
        }
    }

    private async Task OnTableSubmit((int rows, int cols) size)
    {
        _showTableDialog = false;
        if (OnInsertTable.HasDelegate)
        {
            await OnInsertTable.InvokeAsync(size);
        }
    }

    private async Task ToggleSourceView()
    {
        if (OnToggleSourceView.HasDelegate)
        {
            await OnToggleSourceView.InvokeAsync();
        }
    }

    private async Task ToggleFullscreen()
    {
        if (OnToggleFullscreen.HasDelegate)
        {
            await OnToggleFullscreen.InvokeAsync();
        }
    }
}
