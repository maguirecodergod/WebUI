using LHA.BlazorWasm.Components.RichTextEditor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class EditorToolbar : LhaComponentBase
{
    [Parameter] public EditorState State { get; set; } = new();
    [Parameter] public EventCallback<(string command, string? value)> OnCommand { get; set; }
    [Parameter] public EventCallback OnToggleSourceView { get; set; }
    [Parameter] public EventCallback OnToggleFullscreen { get; set; }
    [Parameter] public EventCallback<LinkDialogResult> OnInsertLink { get; set; }
    [Parameter] public EventCallback<ImageDialogResult> OnInsertImage { get; set; }
    [Parameter] public EventCallback<(int rows, int cols)> OnInsertTable { get; set; }
    [Parameter] public EventCallback<CodeBlockResult> OnInsertCodeBlock { get; set; }

    [Parameter] public EventCallback OnImageGalleryRequest { get; set; }

    private ToolbarDropdown? _foreColorDropdown;
    private ToolbarDropdown? _backColorDropdown;
    private ToolbarDropdown? _imageDropdown;
    private ToolbarDropdown? _videoDropdown;
    private ToolbarDropdown? _specialCharsDropdown;
    private ToolbarDropdown? _emojiDropdown;

    private bool _showLinkDialog;
    private bool _showImageDialog;
    private bool _showTableDialog;
    private bool _showDragDropDialog;
    private bool _showCodeBlockDialog;

    private string _uploadInputId = $"upload-{Guid.NewGuid():N}";
    private string _cameraInputId = $"camera-{Guid.NewGuid():N}";
    private string _videoUrl = string.Empty;

    private async Task OnInsertVideoClick()
    {
        if (string.IsNullOrWhiteSpace(_videoUrl)) return;

        var url = _videoUrl.Trim();
        _videoDropdown?.Close();

        // Convert Youtube watch URL to embed URL if necessary
        if (url.Contains("youtube.com/watch"))
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (query["v"] != null)
            {
                url = $"https://www.youtube.com/embed/{query["v"]}";
            }
        }
        else if (url.Contains("youtu.be/"))
        {
            var id = url.Substring(url.LastIndexOf('/') + 1);
            url = $"https://www.youtube.com/embed/{id}";
        }

        var iframeHtml = $"<iframe width=\"560\" height=\"315\" src=\"{url}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";

        await ExecuteCommand("insertHTML", iframeHtml);
        _videoUrl = string.Empty;
    }

    private async Task OnUploadClick()
    {
        _imageDropdown?.Close();
        await JS.InvokeVoidAsync("eval", $"document.getElementById('{_uploadInputId}').click()");
    }

    private async Task OnCameraClick()
    {
        _imageDropdown?.Close();
        await JS.InvokeVoidAsync("eval", $"document.getElementById('{_cameraInputId}').click()");
    }

    private void OnDragDropClick()
    {
        _imageDropdown?.Close();
        _showDragDropDialog = true;
    }

    private async Task OnImageGalleryClick()
    {
        _imageDropdown?.Close();
        if (OnImageGalleryRequest.HasDelegate)
        {
            await OnImageGalleryRequest.InvokeAsync();
        }
    }

    private async Task HandleImageUpload(Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            try
            {
                var resizedFile = await file.RequestImageFileAsync(file.ContentType, 1920, 1080);
                using var memoryStream = new System.IO.MemoryStream();
                await resizedFile.OpenReadStream(maxAllowedSize: 15 * 1024 * 1024).CopyToAsync(memoryStream);
                var buffer = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(buffer);
                var url = $"data:{file.ContentType};base64,{base64}";

                await OnImageSubmit(new ImageDialogResult { Url = url });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing image: {ex.Message}");
            }
        }
    }

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
    private void ShowCodeBlockDialog() => _showCodeBlockDialog = true;

    private async Task OnCodeBlockSubmit(CodeBlockResult result)
    {
        _showCodeBlockDialog = false;
        if (OnInsertCodeBlock.HasDelegate)
        {
            await OnInsertCodeBlock.InvokeAsync(result);
        }
    }

    private async Task OnSpecialCharsSubmit(string chr)
    {
        await ExecuteCommand("insertHTML", chr);
    }

    private async Task OnEmojiSubmit(Emoji.EmojiModel emoji)
    {
        await ExecuteCommand("insertHTML", emoji.Unicode);
        _emojiDropdown?.Close();
    }

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
