using LHA.BlazorWasm.Components.RichTextEditor.Models;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.RichTextEditor.Interop;

/// <summary>
/// C# ↔ JavaScript interop wrapper for the RichTextEditor engine.
/// Manages the JS module lifecycle and provides typed methods.
/// </summary>
public class RichTextEditorInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private DotNetObjectReference<RichTextEditorInterop>? _dotNetRef;

    /// <summary>
    /// Invoked when the editor content changes (debounced).
    /// </summary>
    public event Func<string, Task>? OnContentChanged;

    /// <summary>
    /// Invoked when the selection/format state changes.
    /// </summary>
    public event Func<EditorState, Task>? OnSelectionChanged;

    public RichTextEditorInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/LHA.BlazorWasm.Components/RichTextEditor/js/richTextEditor.js")
            .AsTask());
    }

    /// <summary>
    /// Initialize the editor instance.
    /// </summary>
    public async Task InitEditorAsync(string editorId, EditorOptions options)
    {
        var module = await _moduleTask.Value;
        _dotNetRef = DotNetObjectReference.Create(this);

        await module.InvokeVoidAsync("initEditor", editorId, _dotNetRef, new
        {
            readOnly = options.ReadOnly,
            placeholder = options.Placeholder,
            debounceMs = options.DebounceMs
        });
    }

    /// <summary>
    /// Execute a formatting command on the editor.
    /// </summary>
    public async Task ExecCommandAsync(string editorId, string command, string? value = null)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("execCommand", editorId, command, value);
    }

    /// <summary>
    /// Get the HTML content.
    /// </summary>
    public async Task<string> GetHtmlAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getHtml", editorId);
    }

    /// <summary>
    /// Set the HTML content.
    /// </summary>
    public async Task SetHtmlAsync(string editorId, string html)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setHtml", editorId, html);
    }

    /// <summary>
    /// Get the plain text content.
    /// </summary>
    public async Task<string> GetTextAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getText", editorId);
    }

    /// <summary>
    /// Get the currently selected text.
    /// </summary>
    public async Task<string> GetSelectedTextAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getSelectedText", editorId);
    }

    /// <summary>
    /// Insert HTML at cursor position.
    /// </summary>
    public async Task InsertHtmlAsync(string editorId, string html)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("insertHtml", editorId, html);
    }

    /// <summary>
    /// Insert a code block with syntax highlighting.
    /// </summary>
    public async Task InsertCodeBlockAsync(string editorId, string code, string language)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("insertCodeBlock", editorId, code, language);
    }

    /// <summary>
    /// Insert a table.
    /// </summary>
    public async Task InsertTableAsync(string editorId, int rows, int cols)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("insertTable", editorId, rows, cols);
    }

    /// <summary>
    /// Get format state for toolbar sync.
    /// </summary>
    public async Task<EditorState?> GetFormatStateAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<EditorState?>("getFormatState", editorId);
    }

    /// <summary>
    /// Toggle source code view.
    /// </summary>
    public async Task<bool> ToggleSourceViewAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<bool>("toggleSourceView", editorId);
    }

    /// <summary>
    /// Toggle fullscreen mode.
    /// </summary>
    public async Task<bool> ToggleFullscreenAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<bool>("toggleFullscreen", editorId);
    }

    /// <summary>
    /// Set read-only mode.
    /// </summary>
    public async Task SetReadOnlyAsync(string editorId, bool readOnly)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setReadOnly", editorId, readOnly);
    }

    /// <summary>
    /// Focus the editor content area.
    /// </summary>
    public async Task FocusAsync(string editorId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("focusEditor", editorId);
    }

    // ========== JS → C# Callbacks ==========

    [JSInvokable]
    public async Task OnContentChangedFromJs(string html)
    {
        if (OnContentChanged != null)
        {
            await OnContentChanged.Invoke(html);
        }
    }

    [JSInvokable]
    public async Task OnSelectionChangedFromJs(EditorState state)
    {
        if (OnSelectionChanged != null)
        {
            await OnSelectionChanged.Invoke(state);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();

        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
