using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using LHA.BlazorWasm.Components.RichTextEditor.Models;

namespace LHA.BlazorWasm.Components.RichTextEditor.Components;

public partial class DragDropDialog : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<ImageDialogResult> OnSubmit { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private async Task HandleDropzoneUpload(InputFileChangeEventArgs e)
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
                
                var result = new ImageDialogResult { Url = url };
                
                if (OnSubmit.HasDelegate)
                {
                    await OnSubmit.InvokeAsync(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading dropped image: {ex.Message}");
            }
        }
    }
}
